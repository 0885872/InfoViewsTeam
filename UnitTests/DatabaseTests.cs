using System;
using Xunit;
using Reserveer;
using System.Collections.Generic;

namespace UnitTests
{
    public class DatabaseTests
    {
        public Reserveer.Controllers.Database db;

        public DatabaseTests()
        {
            Reserveer.Controllers.Database database = new Reserveer.Controllers.Database();
            db = database;
        }
        [Fact]
        public void getDomainCheck_ReturnsString_1()
        {

            string result = db.getDomainCheck("hr.nl");
            Assert.Equal(result, "1");

        }

        [Fact]
        public void getGroupAdmin_ReturnsString_NotNull()
        {
            List<string[]> result = db.getGroupAdmin();
            Assert.NotNull(result);

        }
        [Fact]
        public void getGroupRoomReservation_ReturnsString_NotNull()
        {
            List<string[]> result = db.getGroupRoomReservation();
            Assert.NotNull(result);

        }
        [Fact]
        public void getGroupRooms_ReturnsString_NotNull()
        {
            List<string[]> result = db.getGroupRooms();
            Assert.NotNull(result);

        }
        [Fact]
        public void getGroupUser_ReturnsString_NotNull()
        {
            List<string[]> result = db.getGroupUser();
            Assert.NotNull(result);

        }
        [Fact]
        public void getLatestTemperature_ReturnsString_23()
        {
            string[] result = db.getLatestTemperature("1");
            Assert.Equal(result[0], "23.0");



        }
        [Fact]
        public void getReservations_ReturnsString_NotNull()
        {
            List<string[]> result = db.getReservations("1");
            Assert.NotNull(result);

        }
        [Fact]
        public void getRoomName_ReturnsString_H2312()
        {
            string result = db.getRoomName("1");
            Assert.Equal(result, "H2312");

        }
        [Fact]
        public void getRoomReservation_ReturnsString_NotNull()
        {
            List<string[]> result = db.getRoomReservation();
            Assert.NotNull(result);

        }

        [Fact]
        public void getCurrentRoomSensor_ReturnsString_NotNull()
        {
            List<string[]> result = db.getCurrentRoomSensors("1");
            Assert.NotNull(result);

        }

        [Fact]
        public void getUserGroup_ReturnsString_NotNull()
        {
            List<string[]> result = db.getUserGroup();
            Assert.NotNull(result);

        }

        [Fact]
        public void getUserMail_ReturnsString_email()
        {
            string result = db.getUserMail("42");
            Assert.Equal(result, "nee@Hotmail.com");
        }
    }
}
