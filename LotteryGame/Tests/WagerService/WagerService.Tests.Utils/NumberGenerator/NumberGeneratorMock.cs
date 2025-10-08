namespace WagerService.Tests.Utils.NumberGenerator
{
    using Moq;
    
    using WagerService.Core.Contracts;

    public class NumberGeneratorMock
    {
        private readonly Mock<INumberGeneration> numberGenerationMock;

        public INumberGeneration Mock => numberGenerationMock.Object;

        public NumberGeneratorMock()
        {
            numberGenerationMock = new Mock<INumberGeneration>();

            numberGenerationMock.Setup(x => x.Generate()).Returns("1234567890123456789");
        }
    }
}
