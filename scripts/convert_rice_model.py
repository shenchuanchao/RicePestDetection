"""
将下载的水稻病害预训练权重(DenseNet121, 6类)转换为ONNX格式。
模型结构: backbone(DenseNet121, num_classes=0) + mlp_head(1024->1024->512->6)
"""
import torch
import torch.nn as nn
import timm
import os

LABELS = ["白叶枯病", "褐斑病", "健康", "稻瘟病", "叶枯病", "纹枯病"]

script_dir = os.path.dirname(os.path.abspath(__file__))
model_dir = os.path.join(script_dir, "..", "models")
weights_path = os.path.join(model_dir, "rice_disease_detection_models", "best_densenet121_rice.pth")
onnx_path = os.path.join(model_dir, "rice-pest-model.onnx")


class RiceDiseaseModel(nn.Module):
    """匹配plantdoc-predictor的模型结构"""
    def __init__(self, num_classes=6):
        super().__init__()
        self.backbone = timm.create_model('densenet121.ra_in1k', pretrained=False, num_classes=0)
        in_features = self.backbone.num_features  # 1024
        self.mlp_head = nn.Sequential(
            nn.Linear(in_features, 1024),
            nn.ReLU(),
            nn.Linear(1024, 512),
            nn.ReLU(),
            nn.Linear(512, num_classes)
        )

    def forward(self, x):
        features = self.backbone(x)
        return self.mlp_head(features)


# 加载权重
print("创建模型...")
model = RiceDiseaseModel(num_classes=6)

print("加载权重...")
ckpt = torch.load(weights_path, map_location='cpu', weights_only=False)
state_dict = ckpt['state_dict']

# 去掉可能的module.前缀
clean_sd = {}
for k, v in state_dict.items():
    clean_sd[k.replace('module.', '')] = v

model.load_state_dict(clean_sd, strict=True)
model.eval()
print(f"权重加载成功! val_acc={ckpt.get('val_acc', 'N/A')}")

# 导出ONNX（删除旧文件）
for f in [onnx_path, onnx_path + ".data"]:
    if os.path.exists(f):
        os.remove(f)

dummy = torch.randn(1, 3, 224, 224)
print(f"\n导出ONNX: {onnx_path}")
torch.onnx.export(
    model, dummy, onnx_path,
    export_params=True, opset_version=17,
    do_constant_folding=True,
    input_names=["input"], output_names=["output"],
    dynamic_axes={"input": {0: "batch"}, "output": {0: "batch"}}
)

print(f"导出成功! 大小: {os.path.getsize(onnx_path) / 1024 / 1024:.1f} MB")

# 验证
import onnx
onnx_model = onnx.load(onnx_path)
onnx.checker.check_model(onnx_model)
print(f"\n输入: {onnx_model.graph.input[0].name} - {[d.dim_value or d.dim_param for d in onnx_model.graph.input[0].type.tensor_type.shape.dim]}")
print(f"输出: {onnx_model.graph.output[0].name} - {[d.dim_value or d.dim_param for d in onnx_model.graph.output[0].type.tensor_type.shape.dim]}")

# 推理测试
try:
    import onnxruntime as ort
    import math
    sess = ort.InferenceSession(onnx_path, providers=["CPUExecutionProvider"])
    test_input = {sess.get_inputs()[0].name: [[[[0.5]*224]*224]*3]}
    result = sess.run(None, test_input)
    logits = result[0][0]
    exp_vals = [math.exp(x) for x in logits]
    sum_exp = sum(exp_vals)
    probs = [x / sum_exp for x in exp_vals]
    max_idx = probs.index(max(probs))
    print(f"\n推理测试:")
    print(f"  logits: {[f'{x:.3f}' for x in logits]}")
    print(f"  probs:  {[f'{x:.4f}' for x in probs]}")
    print(f"  预测: {LABELS[max_idx]} ({probs[max_idx]:.4f})")
except ImportError:
    print("(onnxruntime未安装，跳过推理测试)")

# 保存标签
with open(os.path.join(model_dir, "rice_labels.txt"), "w", encoding="utf-8") as f:
    for l in LABELS:
        f.write(l + "\n")

print(f"\n完成! 标签: {LABELS}")
