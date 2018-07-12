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
            Assert.Equal("1", result);

        }

        [Fact]
        public void getGroupAdmin_ReturnsString_NotNull()
        {
            List<string[]> result = db.getGroupAdmin();
            Assert.NotNull(result);

        }
 
        [Fact]
        public void getLatestTemperature_ReturnsString_23()
        {
            string[] result = db.getLatestTemperature("1");
            Assert.Equal(result[0], "23.0");
            // Input: id(1), expected output: 23.0, != null



        }
        [Fact]
        public void getReservations_ReturnsString_NotNull()
        {
            List<string[]> result = db.getReservations("1");
            Assert.NotNull(result);
            // Input: id(1), expected output: some value, != null
        }
        [Fact]
        public void getRoomName_ReturnsString_H2312()
        {
            string result = db.getRoomName("1");
            Assert.Equal(result, "H2312");
            //Input: id(1), expected output: H2312 

        }
        [Fact]
        public void getRoomReservation_ReturnsString_NotNull()
        {
            List<string[]> result = db.getRoomReservation("1");
            Assert.NotNull(result);
            //Input: id(1), expected output: some value, != null

        }

        [Fact]
        public void getCurrentRoomSensor_ReturnsString_NotNull()
        {
            List<string[]> result = db.getCurrentRoomSensors("1");
            Assert.NotNull(result);
            //Input: id(1), expected output: some value, != null

        }

        [Fact]
        public void getUserGroup_ReturnsString_NotNull()
        {
            List<string[]> result = db.getUserGroup();
            Assert.NotNull(result);
            //Input: none just method call, expected output: some value, != null
        }

        [Fact]
        public void getUserMail_ReturnsString_email()
        {
            string result = db.getUserMail("42");
            Assert.Equal(result, "nee@Hotmail.com");
            //Input : id(41), expected output: "nee@Hotmail.com"
        }
    }
}
