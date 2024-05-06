namespace Reservations.Tests.Services
{
    using System;
    using System.Linq;

    using FluentAssertions;
    using NUnit.Framework;

    using Reservations.Db;
    using Reservations.Models;
    using Reservations.Services;

    [TestFixture]
    public class ReservationsServiceTests
    {
        private GetAllLectureHallsQuery _mockQueryAllLectureHalls;
        private GetAllLecturersQuery _mockQueryAllLectures;
        private GetAllReservationsQuery _mockQueryAllReservations;
        private AddReservationQuery _mockQueryAddReservation;
        private DeleteReservationQuery _mockQueryDeleteReservation;
        private GetReservationByIdQuery _mockQueryGetReservationById;

        private IDatabase _mockDb;

        [OneTimeSetUp]
        public void Init()
        {
            AutoMapperConfig.Init();
        }

        private ReservationsService CreateReservationsService(bool empty)
        {
            _mockDb = empty ? TestDatabaseFactory.CreateEmptyDatabase() : TestDatabaseFactory.CreateDatabase();

            _mockQueryAllLectureHalls = new GetAllLectureHallsQuery(_mockDb);
            _mockQueryAllLectures = new GetAllLecturersQuery(_mockDb);
            _mockQueryAllReservations = new GetAllReservationsQuery(_mockDb);
            _mockQueryAddReservation = new AddReservationQuery(_mockDb);
            _mockQueryDeleteReservation = new DeleteReservationQuery(_mockDb);
            _mockQueryGetReservationById = new GetReservationByIdQuery(_mockDb);

            return new ReservationsService(_mockQueryAllReservations, _mockQueryGetReservationById,
                _mockQueryAddReservation, _mockQueryDeleteReservation, _mockQueryAllLectures, _mockQueryAllLectureHalls);
        }

        private ReservationsService CreateReservationsServiceForStatisticsTests()
        {
            _mockDb = TestDatabaseFactory.CreateDatabaseForStatistics();

            _mockQueryAllLectureHalls = new GetAllLectureHallsQuery(_mockDb);
            _mockQueryAllLectures = new GetAllLecturersQuery(_mockDb);
            _mockQueryAllReservations = new GetAllReservationsQuery(_mockDb);
            _mockQueryAddReservation = new AddReservationQuery(_mockDb);
            _mockQueryDeleteReservation = new DeleteReservationQuery(_mockDb);
            _mockQueryGetReservationById = new GetReservationByIdQuery(_mockDb);

            return new ReservationsService(_mockQueryAllReservations, _mockQueryGetReservationById,
                _mockQueryAddReservation, _mockQueryDeleteReservation, _mockQueryAllLectures, _mockQueryAllLectureHalls);
        }


        [Test]
        [Category("Task2")]
        public void When_GetByDayRunsAndThereAreReservations_Then_TheyAreReturned()
        {
            // Arrange
            var service = CreateReservationsService(false);

            // Act
            var result = service.GetByDay(new DateTime(2015, 1, 2), 202);

            // Assert
            result.Count().Should().Be(3);
        }


        [Test]
        [Category("Task2")]
        public void When_GetHallsFreeHoursByDayRunsWithPartiallyReservedDay_Then_ProperCollectionIsReturned()
        {
            // Arrange
            var service = CreateReservationsServiceForStatisticsTests();

            // Act
            var result = service.GetHallsFreeHoursByDay(DateTime.Today.AddDays(1)).ToList();

            // Assert
            result.Count().Should().Be(8);
            result.Count(p => p.FreeHoursNumber == 7).Should().Be(1);
            result.Count(p => p.FreeHoursNumber == 8).Should().Be(1);
            result.Count(p => p.FreeHoursNumber == 10).Should().Be(6);
        }


        [Test]
        [Category("TestOnExistingCode")]
        public void When_AllReservationsRunsAgainstEmptyDatabase_Then_EmptyCollectionIsReturned()
        {
            // Arrange
            var service = CreateReservationsService(true);

            // Act
            var allReservations = service.All();

            // Assert
            allReservations.Should().BeEmpty();
        }

        [Test]
        [Category("TestOnExistingCode")]
        public void When_AllReservationsRunsAgainstNotEmptyDatabase_Then_AllReservationsAreReturned()
        {
            // Arrange
            var service = CreateReservationsService(false);

            // Act
            var allReservations = service.All();

            // Assert
            allReservations.Count().Should().Be(3);
        }

        [Test]
        [Category("TestOnExistingCode")]
        public void When_GetReservationByIdRunsAndThatReservationDoesNotExist_Then_NullObjectIsReturned()
        {
            // Arrange
            var service = CreateReservationsService(true);

            // Act
            var reservationById = service.GetById(101);

            // Assert
            reservationById.Should().BeNull();
        }

        [Test]
        [Category("TestOnExistingCode")]
        public void When_GetReservationByIdRunsAndThatReservationExists_Then_ProperReservationIsReturned()
        {
            // Arrange
            var service = CreateReservationsService(false);

            // Act
            var reservationById = service.GetById(2);

            // Assert
            reservationById.Id.Should().Be(2);
            reservationById.LectureHallNumber.Should().Be(202);
        }

        [Test]
        [Category("TestOnExistingCode")]
        public void When_DeleteReservationRunsAndThatReservationDoesNotExist_Then_NothingHappens()
        {
            // Arrange
            var service = CreateReservationsService(true);

            // Act
            service.Delete(1);

            // Assert
            service.All().Should().BeEmpty();
        }

        [Test]
        [Category("TestOnExistingCode")]
        public void When_DeleteReservationRunsAndThatReservationExists_Then_ItIsDeleted()
        {
            // Arrange
            var service = CreateReservationsService(false);

            // Act
            service.Delete(1);

            // Assert
            service.All().Count().Should().Be(2);
            service.GetById(1).Should().BeNull();
            service.GetById(2).Should().NotBeNull();
        }
        [Test]
        [Category("Task2")]
        public void When_GetByDayRunsAndThereAreReservationsButOnDifferentHall_Then_EmptyCollectionIsReturned()
        {
            // Arrange
            var service = CreateReservationsService(false);

            // Act
            var result = service.GetByDay(new DateTime(2015, 1, 2), 201);

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        [Category("Task2")]
        public void When_GetByDayRunsAndThereAreNoReservations_Then_EmptyCollectionIsReturned()
        {
            // Arrange
            var service = CreateReservationsService(false);

            // Act
            var result = service.GetByDay(new DateTime(2012, 8, 1), 101);

            // Assert
            result.Should().BeEmpty();
        }

    }
}
