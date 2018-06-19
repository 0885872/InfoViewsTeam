using System;
using Xunit;
using Reserveer;
using System.Collections.Generic;

namespace UnitTests
{
    public class RaspberryTests
    {
    public Reserveer.Controllers.RaspberryController rb;

    public RaspberryTests()
    {
      Reserveer.Controllers.RaspberryController raspberry = new Reserveer.Controllers.RaspberryController();
      rb = raspberry;
    }
        [Fact]
        public void Test1()
        {


        }    
  }
}
