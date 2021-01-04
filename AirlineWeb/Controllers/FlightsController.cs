using System;
using System.Linq;
using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.MessageBus;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirlineWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController: ControllerBase
    {
        private readonly AirlineDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBus;

        public FlightsController(AirlineDbContext context, IMapper mapper, IMessageBusClient messageBus)
        {
            _context = context;
            _mapper = mapper;
            _messageBus = messageBus;
        }

        [HttpGet("{flightCode}", Name = "GetFlightDetailsByCode")]
        public ActionResult<FlightDetailReadDto> GetFlightDetailsByCode(string flightCode) 
        {
            var flight = _context.FlightDetails.FirstOrDefault(f => f.FlightCode == flightCode);

            if (flight == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<FlightDetailReadDto>(flight));
        }

        [HttpPost]
        public ActionResult<FlightDetailReadDto> CreateFlight(FlightDetailCreateDto flightDetailCreateDto)
        {
            var flight = _context.FlightDetails.FirstOrDefault(f => f.FlightCode == flightDetailCreateDto.FlightCode);

            if (flight == null)
            {
                var flightDetailModel = _mapper.Map<FlightDetail>(flightDetailCreateDto);

                try 
                {
                    _context.FlightDetails.Add(flightDetailModel);
                    _context.SaveChanges();
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                var FlightDetailReadDto = _mapper.Map<FlightDetailReadDto>(flightDetailModel);

                return CreatedAtRoute(nameof(GetFlightDetailsByCode), new {flightCode = FlightDetailReadDto.FlightCode}, FlightDetailReadDto);
            }
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult UpdateFlightDetail(int id, FlightDetailUpdateDto flightDetailUpdateDto) 
        {
            var flight = _context.FlightDetails.FirstOrDefault(f => f.Id == id);

            if (flight == null)
            {
                return NotFound();
            }

            decimal oldPrice = flight.Price;

            _mapper.Map(flightDetailUpdateDto, flight);

            try 
            {
                _context.SaveChanges();

                if (oldPrice != flight.Price)
                {
                    Console.WriteLine("Price changed - Place message on bus");

                    var message = new NotificationMessageDto
                    {
                        WebhookType = "Price Change",
                        OldPrice = oldPrice,
                        // TODO: TYPO
                        NewPrice = flight.Price,
                        FlightCode = flight.FlightCode,
                    };
                    _messageBus.SendMessage(message);
                }
                else 
                {
                    Console.WriteLine("No price change");
                }
                return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}