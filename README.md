# Machine learning models with ONNX and .NET

Most machine learning models are trained in Python using frameworks like PyTorch and TensorFlow. You might expect that an application that runs the trained model would need to have all those frameworks installed. But does it need to? It does not! By converting your model to ONNX, you can run it in any supported language, including your .NET applications! ONNX (Open Neural Network Exchange) is an open standard for machine learning interoperability. It enables tools to work together by allowing them to share models in a commonly understood format. In this presentation we will show you how you can train a machine learning model using transfer learning in PyTorch, we’ll convert the model to ONNX, and we’ll use it in a .NET application.

## Demo: .NET MAUI Sample Application using an ONNX model
In this example you find a .NET MAUI application that takes a picture, runs the picture data throug an ONNX model, show the result on the screen and uses text to speech to speak out the prediction. The example is tested on Android devices.

### Presentation resources
- [Slidedeck](https://speakerdeck.com/hnky/machine-learning-models-with-onnx-and-net)
- [Video Session on .NET Conf 2022](https://www.dotnetconf.net/)

### Resources
- [Dotnet Maui Documentation](https://learn.microsoft.com/en-us/dotnet/maui/get-started/first-app)
- [Learn more about ONNX](https://onnx.ai/)
- [Get started with ORT for C#](https://onnxruntime.ai/docs/get-started/with-csharp.html)

### Speakers
- [Henk Boelman](https://www.henkboelman.com)
- [Bea Stollnitz](https://bea.stollnitz.com/blog/)
