namespace Reservations.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;
    using Db;
    using Models;

    /// <summary>
    /// Implementation of business logic methods concerning reservations
    /// </summary>
    public class ReservationsService : IReservationsService
    {
        private readonly GetAllReservationsQuery _queryAll;
        private readonly GetReservationByIdQuery _queryById;
        private readonly AddReservationQuery _queryAdd;
        private readonly DeleteReservationQuery _queryDelete;

        private readonly GetAllLecturersQuery _queryAllLecturers;
        private readonly GetAllLectureHallsQuery _queryAllLectureHalls;

        public ReservationsService(GetAllReservationsQuery queryAll, GetReservationByIdQuery queryById, AddReservationQuery queryAdd, DeleteReservationQuery queryDelete, GetAllLecturersQuery queryAllLecturers, GetAllLectureHallsQuery queryAllLectureHalls)
        {
            _queryAll = queryAll;
            _queryById = queryById;
            _queryAdd = queryAdd;
            _queryDelete = queryDelete;
            _queryAllLecturers = queryAllLecturers;
            _queryAllLectureHalls = queryAllLectureHalls;
        }

        /// <summary>
        /// Lists all reservations that exist in db
        /// </summary>
        public IEnumerable<ReservationItem> All()
        {
            return Mapper.Map<IEnumerable<ReservationItem>>(_queryAll.Execute().ToList());
        }

        /// <summary>
        /// Gets single reservation by its id
        /// </summary>
        public ReservationItem GetById(int id)
        {
            return Mapper.Map<ReservationItem>(_queryById.Execute(id));
        }

        /// <summary>
        /// Checks whether given reservation can be added.
        /// Performs logical and business validation.
        /// </summary>
        public ValidationResult ValidateNewReservation(NewReservationItem newReservation)
        {
            if (newReservation == null)
            {
                throw new ArgumentNullException("newReservation");
            }

            var result = ValidationResult.Default;

            if (newReservation.From.Date != newReservation.To.Date)
            {
                result |= ValidationResult.MoreThanOneDay;
            }

            if (newReservation.From.Hour >= newReservation.To.Hour)
            {
                result |= ValidationResult.ToBeforeFrom;
            }

            if (newReservation.From.Hour < 8 || newReservation.To.Hour > 18)
            {
                result |= ValidationResult.OutsideWorkingHours;
            }

            if ((newReservation.To - newReservation.From).Hours > 3)
            {
                result |= ValidationResult.TooLong;
            }

            var reservations = _queryAll.Execute().Where(p => p.From.Date == newReservation.From.Date && p.Hall.Number == newReservation.LectureHallNumber).ToList();

            // overlapping
            if (reservations.Any(r => r.To.Hour > newReservation.From.Hour && r.From.Hour <= newReservation.From.Hour) ||
                reservations.Any(r => r.From.Hour < newReservation.To.Hour && r.To.Hour >= newReservation.To.Hour) ||
                reservations.Any(r => r.From.Hour >= newReservation.From.Hour && r.To.Hour <= newReservation.To.Hour))
            {
                result |= ValidationResult.Conflicting;
            }

            // does hall and lecturer exist ?
            if (!_queryAllLecturers.Execute().Any(p => p.Id == newReservation.LecturerId))
            {
                result |= ValidationResult.LecturerDoesNotExist;
            }

            if (!_queryAllLectureHalls.Execute().Any(p => p.Number == newReservation.LectureHallNumber))
            {
                result |= ValidationResult.HallDoesNotExist;
            }

            if (result == ValidationResult.Default)
            {
                result = ValidationResult.Ok; 
            }

            return result;
        }

        /// <summary>
        /// Tries to add given reservation to db, after validating it
        /// </summary>
        public ValidationResult Add(NewReservationItem newReservation)
        {
            if (newReservation == null)
            {
                throw new ArgumentNullException("newReservation");
            }

            var result = ValidateNewReservation(newReservation);
            if ((result & ValidationResult.Ok) == ValidationResult.Ok)
            {
                var reservation = new Reservation
                {
                    From = newReservation.From,
                    To = newReservation.To,
                    Lecturer = _queryAllLecturers.Execute().Single(p => p.Id == newReservation.LecturerId),
                    Hall = _queryAllLectureHalls.Execute().Single(p => p.Number == newReservation.LectureHallNumber),
                };

                _queryAdd.Execute(reservation);
            }

            return result;
        }

        /// <summary>
        /// Deletes (if exists) reservation from db (by its id)
        /// </summary>
        public void Delete(int id)
        {
            _queryDelete.Execute(id);
        }

        /// <summary>
        /// Returns all reservations (listed chronologically) on a given day concerning given hall. 
        /// If a given lecture hall does not exist, throws exception
        /// </summary>
        public IEnumerable<ReservationItem> GetByDay(DateTime day, int hallNumber)
        {
            // TODO
            // Implement method returning all reservations (use _queryAll) on a given day for a given hall.
            // Method should check, whether given hall number exists (use _queryAllLectureHalls for that),
            // and if it's not, throw ArgumentException with some message. 
            // If lecture hall exists, return all reservations in chronological order. 

            var reservations = new List<Reservation>();
            return Mapper.Map<IEnumerable<ReservationItem>>(reservations);
        }

        /// <summary>
        /// Returns statistics (list of pairs [HallNumber, NumberOfFreeHours]) on a given day.
        /// Maximum number of free hours is 10 (working hours are 8-18), minimum 0 of course.
        /// Given day must be from the future (not in the past or today) else the ArgumentException should be thrown.
        /// </summary>
        public IEnumerable<HallFreeHoursStatisticsItem> GetHallsFreeHoursByDay(DateTime day)
        {
            // TODO
            // Implement method returning data described in a method summary.
            // It's important that method should return number of free hours (between 0 and 10) for all
            // lecture halls that exist in the database. Use _queryAll and _queryAllLectureHalls.

            return new List<HallFreeHoursStatisticsItem>();
        }
    }
}