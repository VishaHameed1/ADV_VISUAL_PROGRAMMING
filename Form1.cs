using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace SentimentAi
{
    public partial class Form1 : Form
    {
        private MLContext _mlContext;
        private ITransformer _model;

        public Form1()
        {
            InitializeComponent();
            InitializeAI(); // App load hotay hi AI train hogi
        }

        private void InitializeAI()
        {
            try
            {
                _mlContext = new MLContext();

                // Expanded Training Data for better accuracy
                var data = new List<InputData>
                {
                    new InputData { Text = "I love this!", Sentiment = true },
                    new InputData { Text = "This is great.", Sentiment = true },
                    new InputData { Text = "I am feeling happy", Sentiment = true },
                    new InputData { Text = "Good job and well done.", Sentiment = true },
                    new InputData { Text = "This is wonderful.", Sentiment = true },
                    new InputData { Text = "I am stressed.", Sentiment = false },
                    new InputData { Text = "This is bad.", Sentiment = false },
                    new InputData { Text = "I hate this.", Sentiment = false },
                    new InputData { Text = "I feel overwhelmed.", Sentiment = false },
                    new InputData { Text = "Terrible experience.", Sentiment = false }
                };

                var trainingData = _mlContext.Data.LoadFromEnumerable(data);

                // ML Pipeline
                var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", "SentimentText")
                    .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

                _model = pipeline.Fit(trainingData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("AI Initialization Error: " + ex.Message);
            }
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text))
            {
                MessageBox.Show("Please enter some text to analyze!");
                return;
            }

            try
            {
                var engine = _mlContext.Model.CreatePredictionEngine<InputData, Prediction>(_model);
                var result = engine.Predict(new InputData { Text = txtInput.Text });

                // UI Update logic
                if (result.PredictionResult)
                {
                    lblResult.Text = "Analysis: 😊 Positive";
                    lblResult.ForeColor = Color.Green;
                }
                else
                {
                    lblResult.Text = "Analysis: ☹️ Negative";
                    lblResult.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Analysis Error: " + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e) { }
    }

    // Moved these classes to the bottom to fix the Designer Error
    public class InputData
    {
        [ColumnName("SentimentText")] public string Text { get; set; }
        [ColumnName("Label")] public bool Sentiment { get; set; }
    }

    public class Prediction
    {
        [ColumnName("PredictedLabel")] public bool PredictionResult { get; set; }
    }
}