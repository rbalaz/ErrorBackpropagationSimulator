namespace ErrorBackpropagationMVC
{
    public class ErrorBackpropagation
    {
        private string layers;
        private int inputs;
        public int outputs { get; set; }
        private string fileName;
        private string folderPath;
        private double learningParameter;
        private double tolerance;
        private double learningSuccessRate;

        public ErrorBackpropagation(string layers, string fileName, double learningParameter, double tolerance, double learningSuccessRate, string folderPath)
        {
            this.layers = layers;
            this.fileName = fileName;
            this.learningParameter = learningParameter;
            this.tolerance = tolerance;
            this.learningSuccessRate = learningSuccessRate;
            this.folderPath = folderPath;
        }

        public ErrorBackpropagation(string fileName)
        {
            this.fileName = fileName;
        }

        public void execute(out double[] learningRate, out double[] errorRate, out double[][] learningTable, out double[][] testingTable)
        {
            DataLoader loader = new DataLoader(fileName);
            Data[] data = loader.loadDataFromFile();
            inputs = loader.inputs;
            outputs = loader.outputs;

            Initialiser init = new Initialiser(layers, inputs, 0.75, outputs);
            Network network = init.createMultiHiddenLayerNetwork();

            Learning learning = new Learning(learningParameter, data, network, tolerance);
            learning.folderPath = folderPath;
            learning.executeLearningCycle(learningSuccessRate);

            learningRate = learning.learningRate.ToArray();
            errorRate = learning.globalErrorRate.ToArray();
            learningTable = learning.learningTable;
            testingTable = learning.testingTable;
        }

        public void testNetworkFromFile(out double[][] learningTable, out double[][] testingTable, out Network network)
        {
            Initialiser init = new Initialiser();

            Data[] trainingData;
            Data[] testingData;
            network = init.loadNetworkFromFile(fileName, out trainingData, out testingData);

            Learning learning = new Learning(trainingData, testingData, network);
            learning.retestTrainedNetwork();

            testingTable = learning.testingTable;
            learningTable = learning.learningTable;

        }
            
    }
}