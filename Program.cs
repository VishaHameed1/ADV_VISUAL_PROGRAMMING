using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;

// 1. Data Models define karein
public class InputData
{
    [ColumnName("SentimentText")]
    public string Text { get; set; }

    [ColumnName("Label")]
    public bool Sentiment { get; set; }
}

public class Prediction
{
    [ColumnName("PredictedLabel")]
    public bool PredictionResult { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        var mlContext = new MLContext();

        // 2. Dummy Data (Training ke liye)
        var data = new List<InputData>
        {
            new InputData { Text = "I love this project!", Sentiment = true },
            new InputData { Text = "This is very bad and slow.", Sentiment = false },
            new InputData { Text = "Great experience working with .NET", Sentiment = true },
            new InputData { Text = "I hate bugs in my code.", Sentiment = false },
            new InputData { Text = "Everything is perfect.", Sentiment = true },
            new InputData { Text = "This is a terrible mistake.", Sentiment = false }
        };

        var trainingData = mlContext.Data.LoadFromEnumerable(data);

        // 3. Pipeline setup
        var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", "SentimentText")
            .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

        // 4. Model Train karein
        var model = pipeline.Fit(trainingData);

        // 5. User se input lein
        Console.WriteLine("\n=====================================");
        Console.WriteLine("   AI SENTIMENT ANALYZER (.NET 8/10) ");
        Console.WriteLine("=====================================");
        Console.Write("\nShare your thoughts: ");

        string userInput = Console.ReadLine();

        if (!string.IsNullOrEmpty(userInput))
        {
            var engine = mlContext.Model.CreatePredictionEngine<InputData, Prediction>(model);
            var result = engine.Predict(new InputData { Text = userInput });

            string mood = result.PredictionResult ? "😊 Positive" : "☹️ Negative";
            Console.WriteLine($"\nAI Analysis: {mood}");
        }

        Console.WriteLine("\nExit karne ke liye koi key dabayein...");
        Console.ReadKey();
    }
}