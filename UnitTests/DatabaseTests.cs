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
        public void Test1()
        {
      
          string result = db.getDomainCheck("hr.nl");
          Assert.Equal(result, "1");

        }

      [Fact]
      public void Test2()
    {
      List<string[]> result = db.getGroupAdmin();
      Assert.Equal(result, null);

    }
    [Fact]
    public void Test3()
    {
      List<string[]> result = db.getGroupRoomReservation();
      Assert.Equal(result, null);

    }
    [Fact]
    public void Test4()
    {
      List<string[]> result = db.getGroupRooms();
      Assert.Equal(result, null);

    }
    [Fact]
    public void Test5()
    {
      List<string[]> result = db.getGroupUser();
      Assert.Equal(result, null);

    }
    [Fact]
    public void Test6()
    {
      string[] result = db.getLatestTemperature("1");
      Assert.Equal(result, null);



    }
    [Fact]
    public void Test7()
    {
      List<string[]> result = db.getReservations("1");
      Assert.Equal(result, null);

    }
    [Fact]
    public void Test8()
    {
      string result = db.getRoomName("1");
      Assert.Equal(result, "H2312");

    }
    [Fact]
    public void Test9()
    {
      List<string[]> result = db.getRoomReservation();
      Assert.Equal(result, null);

    }

    [Fact]
    public void Test10()
    {
      List<string[]> result = db.getRoomSensors("1");
      Assert.Equal(result, null);

    }

    [Fact]
    public void Test11()
    {
      List<string[]> result = db.getUserGroup();
      Assert.Equal(result, null);

    }

    [Fact]
    public void Test12()
    {
      string result = db.getUserMail("42");
      Assert.Equal(result, "nee@Hotmail.com");


    }

    //[Fact]
    //public void Test13()
    //{
    //  db.setReservations();

    //}

    //[Fact]
    //public void Test14()
    //{
    //  db.VerifyMail();

    //}      
  }
}
