using ZCL.Calculator.Impl;

namespace ZCL.Calculator
{
    public class CalculatorFactory
    {
        private readonly CommandProvider commandProvider;

        public CalculatorFactory()
            : this(new CommandConfigurer())
        { }

        public CalculatorFactory(CommandConfigurer commandConfigurer)
        {
            commandProvider = new CommandProvider(commandConfigurer);
        }


        public ICalculator createInstance() {
            return new CalculatorImpl(commandProvider);
        }

    }


}
