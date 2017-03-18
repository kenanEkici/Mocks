using System;
using MoqKoans.KoansHelpers;
using NUnit.Framework;

namespace MoqKoans
{
	/// <summary>
	/// The tests in this class will make you familiar with creating Mock objects with Moq.
	/// </summary>
	[TestFixture]
	public class Moq1_Interfaces : Koan
	{
		// This is an interface that we will be mocking.
		public interface IVolume
		{
			void Louder();
			void Quieter();
			string Current();
		}

		// This is a simple implementation of IVolume.
		private class Volume : IVolume
		{
			private int volume = 50;

			public void Louder() { volume++; }
			public void Quieter() { volume--; }
			public string Current() { return volume.ToString(); }
		}

		[Test]
		public void VariablesStartAsNull()
		{
			IVolume volume = null;
			Assert.AreEqual(true, volume == null);
		}

		[Test]
		public void AnInstanceOfVolumeIsAlsoAnIVolume()
		{
			var volume = new Volume();
			Assert.AreEqual(true, volume is IVolume);
		}

		[Test]
		public void AMockOfIVolumeIsNotAnIVolume()
		{
			var volume = new Moq.Mock<IVolume>();
			Assert.AreEqual(false, volume is IVolume);
			Assert.IsTrue(volume is Moq.Mock);
		}

		[Test]
		public void TheObjectPropertyOfAMockReturnsAnInstanceThatImplementsTheMockedInterface()
		{
			var volume = new Moq.Mock<IVolume>();
			Assert.AreEqual(true, volume.Object is IVolume);
		}

		// IVolume was a public interface.
		// This one is private instead.
		private interface IPrivateInterface
		{
			string SomeMethod();
		}

		[Test]
		public void CanNotMockAPrivateInterface()
		{
			var throwsException = false;
			try
			{
				var mock = new Moq.Mock<IPrivateInterface>().Object;
			}
			catch (Exception)
			{
				throwsException = true;
			}

			Assert.AreEqual(true, throwsException);
		}

		[Test]
		public void CreateANewMockOfIVolumeToMakeThisTestPass()
		{
			var mock = new Moq.Mock<IVolume>();
            Assert.That(mock, Is.InstanceOf<Moq.Mock<IVolume>>());
        }
	}
}
