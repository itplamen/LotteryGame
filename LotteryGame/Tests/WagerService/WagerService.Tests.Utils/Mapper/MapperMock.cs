namespace WagerService.Tests.Utils.Mapper
{
    using AutoMapper;

    using Moq;

    using WagerService.Core.Models;
    using WagerService.Data.Models;

    public class MapperMock
    {
        private readonly Mock<IMapper> mapperMock;

        public IMapper Mock => mapperMock.Object;

        public MapperMock()
        {
            mapperMock = new Mock<IMapper>();

            mapperMock
                .Setup(x => x.Map<IEnumerable<TicketDto>>(It.IsAny<IEnumerable<Ticket>>()))
                .Returns((IEnumerable<Ticket> tickets) =>
                    tickets.Select(x => new TicketDto()
                    {
                        Id = x.Id,
                        Status = x.Status,
                        TicketNumber = x.TicketNumber 
                    }));
        }
    }
}
