## Introduction
  
This reservations project is about exposing a WEB API for the purposes of lecture halls at a university. There is a given set
of subjects, lecturers and lecture halls. One lecturer is assigned to one subject, one subject can have many lecturers. 
Reservations are linked to a specific lecturer in a specific lecture hall and timeframe. The API allows listing lecturers, 
subjects and lecture halls - there is no possibility to manipulate them. The Project concentrates on reservations - 
we can list/add/delete and get basic statistics about them.
 
Ninject is used to resolve dependencies and query pattern for data retrieval. A Simple, custom, in-memory database is used 
as a storage mechanism for the purposes of the project. Other packages used: AutoMapper (mapping between entities and
view models), NUnit and FluentAssertions for unit tests.

## Problem Statement
  

Your job is to implement two methods in `ReservationsService` - `GetByDay` and `GetHallsFreeHoursByDay`. 

 - `GetByDay` - Implement a method returning all reservations (use `_queryAll`) on a given day for a given hall. The method should check, whether a given hall number exists (use `_queryAllLectureHalls` for that), and if it doesn't, it should throw `ArgumentException` with some message. If the lecture hall exists, return all reservations in chronological order. 
 - `GetHallsFreeHoursByDay` -  Implement a method returning data described in a method summary. It's important that a method should return the number of free hours (between 0 and 10) for all lecture halls that exist in the database. Use `_queryAll` and `_queryAllLectureHalls`.

## Hints

To **run tests** on your local environment, you may be required to run them as `administrator` or/and in Visual Studio go to `Tools` | `Options` | `Test` | `General` and uncheck the `For improved performance, only use test adapters in test assembly folders or as specified in runsettings file` checkbox.