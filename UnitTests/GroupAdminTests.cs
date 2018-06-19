using System;
using Xunit;
using Reserveer;
using System.Collections.Generic;

namespace UnitTests
{
    public class GroupAdminTests
    {
    public Reserveer.Controllers.GroupsAdminController ad;

    public GroupAdminTests()
    {
      Reserveer.Controllers.GroupsAdminController admin = new Reserveer.Controllers.GroupsAdminController();
      ad = admin;
    }
        [Fact]
        public void Deactivatuser_ReturnsString_NotNull()
        {
      var result = ad.DeactivateUser("1","42");
      Assert.NotNull(result);

        }

      [Fact]
      public void DeleteReservation_ReturnsString_NotNull()
    {
      var result = ad.DeleteReservation("60" , "1");
      Assert.NotNull(result);

    } 
  }
}
