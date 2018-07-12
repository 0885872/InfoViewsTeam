using System;
using Xunit;
using Reserveer;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests
{
    public class GroupAdminTests // Tests results from the GroupsAdminController
    {
    public Reserveer.Controllers.GroupsAdminController ad;

    public GroupAdminTests()
    {
      Reserveer.Controllers.GroupsAdminController admin = new Reserveer.Controllers.GroupsAdminController();
      ad = admin;
    }

    [Fact]
    public void Addroom_ReturnsView_NotNull()
    {
      var result = ad.AddRoom();
      Assert.NotNull(result);
            //Input: None, just a method call. Expected output: some value, != null

    }


    [Fact]
    public void Deactivatuser_ReturnsString_NotNull()
    {
    var result = ad.DeactivateUser("1","42");
    Assert.NotNull(result);
            //Input: 2x id(1,42), expected output: some value, != null

    }

    [Fact]
    public void DeleteReservation_ReturnsString_NotNull()
    {
    var result = ad.DeleteReservation("60" , "1");
    Assert.NotNull(result);
            //Input: 2x id(60,1), expected output: some value, != null

        }

    }
}
