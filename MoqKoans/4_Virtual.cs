using System;
using NUnit.Framework;
using Moq;
using MoqKoans.KoansHelpers;

namespace MoqKoans
{
	[TestFixture]
	public class Moq4_Virtual : Koan
	{
		// A concrete class to test Moq with.
		// Note that this class contains a virtual method.
		public class Vehicle
		{
			public int GetHorsepower() { return 1001; }
			public virtual int GetNumerOfWheels() { return 4; }
		}

		[Test]
		public void MoqCanCreateMocksAroundConcreteClasses()
		{
			// note that Vehicle is not an interface.
			var mockVehicle = new Mock<Vehicle>();

			Assert.AreEqual(true, mockVehicle.Object is Vehicle);
		}
		
		[Test]
		public void NonVirtualMethodsAreWrappered_TheBaseMethodIsStillCalled()
		{
			var mockVehicle = new Mock<Vehicle>();

			Assert.AreEqual(1001, mockVehicle.Object.GetHorsepower());
		}
		
		[Test]
		public void VirtualMethodsAreHandledByMoq()
		{
			var mockVehicle = new Mock<Vehicle>();

			Assert.AreEqual(0, mockVehicle.Object.GetNumerOfWheels());
		}

		[Test]
		public void VirtualMethodsCanBeSetupTheSameAsInterfaces()
		{
			var mockVehicle = new Mock<Vehicle>();
			mockVehicle.Setup(m => m.GetNumerOfWheels()).Returns(18);

			Assert.AreEqual(18, mockVehicle.Object.GetNumerOfWheels());			
		}

		[Test]
		public void NonVirtualMethodsCanNotBeSetup()
		{
			var mockVehicle = new Mock<Vehicle>();
			var exceptionWasThrown = false;
			try
			{
				mockVehicle.Setup(m => m.GetHorsepower()).Returns(170);
			}
			catch (Exception)
			{
				exceptionWasThrown = true;
			}
			Assert.AreEqual(true, exceptionWasThrown);
		}

		// An abstract class to test Moq with.
		public abstract class Person
		{
			public abstract string GetFirstName();
			public abstract string GetLastName();
		}

		[Test]
		public void MoqCanMockAbstractClasses()
		{
			var mock = new Mock<Person>();

            Assert.That(mock.Object, Is.InstanceOf(typeof(Person)));
        }

		[Test]
		public void WithLooseBehaviorMockedAbstractMethodsReturnTheirDefaultValue()
		{
			var mock = new Mock<Person>(MockBehavior.Loose);

			Assert.AreEqual(null, mock.Object.GetFirstName());
		}

		[Test]
		public void WithStrictBehaviorMockedAbstractMethodsThrowAnExceptionUnlessSetup()
		{
			var mock = new Mock<Person>(MockBehavior.Strict);
			mock.Setup(x => x.GetFirstName()).Returns("Fred");

			Assert.AreEqual("Fred", mock.Object.GetFirstName());
			var exceptionWasThrown = false;
			try
			{
				mock.Object.GetLastName();
			}
			catch (Exception)
			{
				exceptionWasThrown = true;
			}
			Assert.AreEqual(true, exceptionWasThrown);
		}

		[Test]
		public void SetupAMockPersonWithTheNameJohnDoe()
		{
			var mock = new Mock<Person>(MockBehavior.Strict);
			mock.Setup(m=>m.GetFirstName()).Returns("John");
			mock.Setup(m=>m.GetLastName()).Returns("Doe");

			var person = mock.Object;
			Assert.AreEqual("John", person.GetFirstName());
			Assert.AreEqual("Doe", person.GetLastName());
		}
	}
}
