
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using ONNX_Maui_App.Models;
using IImage = Microsoft.Maui.Graphics.IImage;
using SixLabors.ImageSharp.Formats;
using Microsoft.Maui.Graphics.Platform;

namespace ONNX_Maui_App;

public partial class MainPage : ContentPage
{
    private List<string> _classNames { get; set; }

    private InferenceSession _session { get; set; }
  
    public MainPage()
	{
		InitializeComponent();
        _classNames = LoadLabels();
        _session = LoadModel();
    }

    private InferenceSession LoadModel() 
    {
        using var modelStream = FileSystem.OpenAppPackageFileAsync("model.onnx").Result;

        using var modelMemoryStream = new MemoryStream();
        modelStream.CopyTo(modelMemoryStream);

        var _model = modelMemoryStream.ToArray();
        InferenceSession inferenceSession = new InferenceSession(model:_model);

        return inferenceSession;
    }

    private List<string> LoadLabels()
    {
        // Loading the labels
        using var stream = FileSystem.OpenAppPackageFileAsync("labels.txt").Result;
        using var reader = new StreamReader(stream);

        List<string> labels = new List<string>();
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            labels.Add(line);
        }
        return labels;
    }

    private Prediction Predict(IImage originalImage) {

        // Transform Image
        using SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgb24> image = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgb24>(originalImage.AsStream(), out IImageFormat format);

        // Preprocess image
        Tensor<float> input = new DenseTensor<float>(new[] { 1, 3, 224, 224 });
        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                Span<SixLabors.ImageSharp.PixelFormats.Rgb24> pixelSpan = accessor.GetRowSpan(y);
                for (int x = 0; x < accessor.Width; x++)
                {
                    input[0, 0, y, x] = ((pixelSpan[x].R / 255f)); 
                    input[0, 1, y, x] = ((pixelSpan[x].G / 255f));
                    input[0, 2, y, x] = ((pixelSpan[x].B / 255f));
                }
            }
        });
       
        // Setup inputs
        var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input.1", input)
            };

        // Run inference
        using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = _session.Run(inputs);

        // Postprocess to get softmax vector
        IEnumerable<float> output = results.First().AsEnumerable<float>();
        float sum = output.Sum(x => (float)Math.Exp(x));
        IEnumerable<float> softmax = output.Select(x => (float)Math.Exp(x) / sum);

        // Extract top 10 predicted classes
        IEnumerable<Prediction> top10 = softmax.Select((x, i) => new Prediction { Label = _classNames[i], Confidence = x })
                           .OrderByDescending(x => x.Confidence)
                           .Take(10);

        return top10.FirstOrDefault();
    }

    private async void TakePhoto(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                // Get the image
                using Stream sourceStream = await photo.OpenReadAsync();
                IImage image = PlatformImage.FromStream(sourceStream);

                // Crop the image to 224 224 pixels
                IImage newImage = image.Resize(224, 224, ResizeMode.Bleed);

                // Classify the image
                Prediction prediction = Predict(newImage);

                // Show the result
                ImageCanvas.Source = ImageSource.FromStream(() => newImage.AsStream());
                Result.Text = prediction.Label;
                Accuracy.Text = $"Confidence: {prediction.Confidence*100:0.00}%";

                // Speak the result
                await TextToSpeech.Default.SpeakAsync(($"{prediction.Label}, {prediction.Confidence * 100:0.00}% sure"));
            }
        }
    }
}

