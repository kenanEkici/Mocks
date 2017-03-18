using System;
using NUnit.Framework;
using Moq;
using MoqKoans.KoansHelpers;

namespace MoqKoans
{
	[TestFixture]
	public class Moq2_Methods : Koan
	{
		// This is an interface that we will be mocking.
		public interface IVolume
		{
			int Louder(int amount);
			int Quieter(int amount);
			string CurrentVolume();
		}

		[Test]
		public void IfAMockIsCreatedWithTheLooseBehaviorThenAllMethodsReturnTheirDefaultValues()
		{
			var volumeMock = new Mock<IVolume>(MockBehavior.Loose); 
			var volume = volumeMock.Object;

			Assert.AreEqual(null, volume.CurrentVolume());
			Assert.AreEqual(0, volume.Louder(0));
			Assert.AreEqual(0, volume.Quieter(0));
		}

		[Test]
		public void IfNotSpecifiedTheDefaultBehaviorIsLoose()
		{
			var volumeMock = new Mock<IVolume>();
			var volume = volumeMock.Object;

			Assert.AreEqual(null, volume.CurrentVolume());
			Assert.AreEqual(0, volume.Louder(0));
			Assert.AreEqual(0, volume.Quieter(0));
		}

		[Test]
		public void CanPassAnyValueToAMockMethod()
		{
			IVolume volume = new Mock<IVolume>().Object;

			Assert.AreEqual(0, volume.Louder(0));
			Assert.AreEqual(0, volume.Louder(50));
			Assert.AreEqual(0, volume.Louder(-12));
		}

		[Test]
		public void IfAMockIsCreatedWithTheStrictBehaviorThenAllMethodsThrowAnExceptionIfCalled()
		{
			var volumeMock = new Mock<IVolume>(MockBehavior.Strict);
			var volume = volumeMock.Object;

			var exceptionWasThrown = false;
			try
			{
				volume.CurrentVolume();
			}
			catch (Exception)
			{
				exceptionWasThrown = true;
			}
			Assert.AreEqual(true, exceptionWasThrown);
		}

		[Test]
		public void TheSetupMethodChangesTheBehaviorOfAMockedMethod()
		{
			var volumeMock = new Mock<IVolume>(MockBehavior.Strict);
			var volume = volumeMock.Object;

			// This tells Moq that the CurrentVolume() method should return string.Empty when called.
			volumeMock.Setup(m => m.CurrentVolume()).Returns(string.Empty);

			var exceptionWasThrown = false;
			try
			{
				volume.CurrentVolume();
			}
			catch (Exception)
			{
				exceptionWasThrown = true;
			}
			Assert.AreEqual(false, exceptionWasThrown);
		}

		[Test]
		public void TheSetupMethodCanSpecifyAReturnValueForTheMethod()
		{
			var mock = new Mock<IVolume>(MockBehavior.Strict);
			mock.Setup(m => m.CurrentVolume()).Returns("100");

			Assert.AreEqual("100", mock.Object.CurrentVolume());
		}

		[Test]
		public void WriteASetupMethodToMakeCurrentVolumeReturnTheExpectedValue()
		{
			var mock = new Mock<IVolume>();
			mock.Setup(m => m.CurrentVolume()).Returns("yay!");

			Assert.AreEqual("yay!", mock.Object.CurrentVolume());
		}


		[Test]
		public void MultipleSetupMethodsForTheSameMethodUsesTheLastOneRun()
		{
			var mock = new Mock<IVolume>();
			mock.Setup(m => m.CurrentVolume()).Returns("10");
			mock.Setup(m => m.CurrentVolume()).Returns("50");

			Assert.AreEqual("50", mock.Object.CurrentVolume());
		}

		[Test]
		public void TheValuePassedToReturnsIsEvaluatedWhenReturnsIsCalledNotWhenTheMethodIsCalled()
		{
			// this behavior often trips up new users to Moq!

			var currentVolume = 50;
			var mock = new Mock<IVolume>();

			mock.Setup(m => m.CurrentVolume()).Returns(currentVolume.ToString());

			Assert.AreEqual("50", mock.Object.CurrentVolume()); //when return is called

			currentVolume = 10; //need to be this value when setup

			Assert.AreEqual("50", mock.Object.CurrentVolume()); //when method is called
		}

		[Test]
		public void TheReturnsMethodCanAlsoBeGivenADelegateOrLambdaToEvaluateEachTimeTheMethodIsCalled()
		{
			var currentVolume = 50;
			var mock = new Mock<IVolume>();

			mock.Setup(m => m.CurrentVolume()).Returns(() => currentVolume.ToString());

			Assert.AreEqual("50", mock.Object.CurrentVolume());

			currentVolume = 10;

			Assert.AreEqual("10", mock.Object.CurrentVolume());
			
			// Ask yourself; why does this behave differently than the previous test?

		}

		[Test]
		public void LambdasPassedToReturnsCanDoComplicatedThings()
		{
			var currentVolume = 50;
			var mock = new Mock<IVolume>();

			mock.Setup(m => m.Louder(It.IsAny<int>()))
				.Returns<int>(input =>
				{
					var newVolume = currentVolume + input;
					if (newVolume > 50)
						return 100;
					return 0;
				});

			Assert.AreEqual(100, mock.Object.Louder(10));			
			Assert.AreEqual(0, mock.Object.Louder(-10));			
		}

		[Test]
		public void WhenAMethodTakesInputParametersTheSetupMethodCanHandleThem_ItIsAny_MatchesAllValues()
		{
			var mock = new Mock<IVolume>();
			// This call to .Setup() tells Moq that when any int is passed to Louder(), return 10.
			mock.Setup(m => m.Louder(It.IsAny<int>())).Returns(10);

			Assert.AreEqual(10, mock.Object.Louder(0));
			Assert.AreEqual(10, mock.Object.Louder(50));
			Assert.AreEqual(10, mock.Object.Louder(-2));
		}

		[Test]
		public void ItIs_CanUseASpecificValue()
		{
			var mock = new Mock<IVolume>();
			mock.Setup(m => m.Louder(10)).Returns(10); //input must be 10 or else it returns 0

			Assert.AreEqual(10, mock.Object.Louder(10));
			Assert.AreEqual(0, mock.Object.Louder(50));
		}

		[Test]
		public void MultipleSetupMethodsCanTakeDifferentParameters()
		{
			var mock = new Mock<IVolume>();
			mock.Setup(m => m.Louder(1)).Returns(10);
			mock.Setup(m => m.Louder(2)).Returns(20);

			Assert.AreEqual(10, mock.Object.Louder(1));
			Assert.AreEqual(20, mock.Object.Louder(2));
		}

		[Test]
		public void ItIs_CanTakeLambdaExpressionAsAParameterMatchFilter()
		{
			var mock = new Mock<IVolume>();
			mock.Setup(m => m.Louder(It.Is<int>(p => p >= 0))).Returns(10);
			mock.Setup(m => m.Louder(It.Is<int>(p => p < 0))).Returns(-10);

			Assert.AreEqual(10, mock.Object.Louder(5));
			Assert.AreEqual(-10, mock.Object.Louder(-2));
		}

		[Test]
		public void SetupTheMockQuieterMethodToReturnTheDesiredResultsToMakeTheTestPass()
		{
			var mock = new Mock<IVolume>();
			mock.Setup(m => m.Quieter(It.Is<int>(p=> p < 0))).Returns(0);
			mock.Setup(m => m.Quieter(It.Is<int>(p => p > 0))).Returns(100);

			Assert.AreEqual(0, mock.Object.Quieter(-2));
			Assert.AreEqual(0, mock.Object.Quieter(-1));
			Assert.AreEqual(100, mock.Object.Quieter(1));
			Assert.AreEqual(100, mock.Object.Quieter(2));
		}

		[Test]
		public void SetupCanReturnValuesFromVariables()
		{
			var someObject = new { ReturnValue = "I Am A Return Value!" };
		    
			var mock = new Mock<IVolume>();
			mock.Setup(x => x.CurrentVolume()).Returns(someObject.ReturnValue);

			Assert.AreEqual(someObject.ReturnValue, mock.Object.CurrentVolume());
		}

		[Test]
		public void SetupCanReturnThePassedInParameter()
		{
			var mock = new Mock<IVolume>();
			var volume = mock.Object;
			mock.Setup(m => m.Louder(It.IsAny<int>())).Returns<int>(p => p);
			// The <int> generic on .Returns() tells it the value of the parameter being passed in from the Louder() method.

			Assert.AreEqual(1, volume.Louder(1));
			Assert.AreEqual(5, volume.Louder(5));
			Assert.AreEqual(10, volume.Louder(10));
			Assert.AreEqual(20, volume.Louder(20));
		}

		[Test]
		public void WriteASingleSetupMethodForQuieterSoThatItAlwaysReturnsOneLessThanThePassedInValue()
		{
			var mock = new Mock<IVolume>();
			var volume = mock.Object;
			mock.Setup(m => m.Quieter(It.IsAny<int>())).Returns<int>(p=> p -= 1);

			Assert.AreEqual(0, volume.Quieter(1));
			Assert.AreEqual(1, volume.Quieter(2));
			Assert.AreEqual(2, volume.Quieter(3));
		}

		// This interface has a method that takes more than 1 parameter.
		public interface IAddition
		{
			int Add(int left, int right);
		}

		[Test]
		public void CanWorkWithMultipleParameters()
		{
			var mock = new Mock<IAddition>();
			mock.Setup(m => m.Add(It.IsAny<int>(), It.IsAny<int>())).Returns<int, int>((left, right) => left + right);

			Assert.AreEqual(3, mock.Object.Add(1, 2));
			Assert.AreEqual(10, mock.Object.Add(5, 5));
		}

		[Test]
		public void SetupMethodsCanBeToldThThrowAnExceptionWhenCalled()
		{
			var mock = new Mock<IVolume>();
			mock.Setup(x => x.CurrentVolume()).Throws(new InvalidOperationException("Calling CurrentVolume() will throw this Exception."));

			var exceptionWasThrown = false;
			try
			{
				mock.Object.CurrentVolume();
			}
			catch (Exception)
			{
				exceptionWasThrown = true;
			}
			Assert.AreEqual(true, exceptionWasThrown);
		}

		[Test]
		public void CreateAMockIAdditionThatOnlyAddsPositiveNumbersAndThrowsAnExceptionIfEitherNumerIsNegative()
		{
			// hint: remember that MockBehavior.Strict will cause an Exception if the parameters don't match any .Setup() filters.
			var mock = new Mock<IAddition>(MockBehavior.Strict);
		    mock.Setup(m => m.Add(It.Is<int>(p => p >= 0), It.Is<int>(p => p >= 0))).Returns<int,int>((left,right) => left + right);

			Assert.AreEqual(3, mock.Object.Add(1, 2));
			Assert.AreEqual(10, mock.Object.Add(0, 10));
            try
            {
                mock.Object.Add(5, -5);
                Assert.Fail("The .Add() method did not throw an Exception when a negative number was passed in.");
            }
            catch (Exception)
            {
                Assert.Pass();
            }

        }

		[Test]
		public void SetupMethodsCanExecuteADelegateOrLambdaWhenCalled()
		{
		    bool louderWasCalled = false;
            bool quieterWasCalled = false;

			var mock = new Mock<IVolume>();
			mock.Setup(x => x.Louder(It.IsAny<int>())).Callback(() => louderWasCalled = true);
			mock.Setup(x => x.Quieter(It.IsAny<int>())).Callback(() => quieterWasCalled = true);

			mock.Object.Louder(5);

			Assert.AreEqual(true, louderWasCalled);
			Assert.AreEqual(false, quieterWasCalled);
		}

		[Test]
		public void SetupMethodCanPerformAnActionAndReturnAValue()
		{
			bool louderWasCalled = false;

			// In this form, .Callback() performs an action and .Returns() sets a return value.
			var mock = new Mock<IVolume>();
			mock.Setup(x => x.Louder(It.IsAny<int>()))
				.Callback(() => louderWasCalled = true)
				.Returns<int>(input => input);

			var result = mock.Object.Louder(5);

			Assert.AreEqual(true, louderWasCalled);
			Assert.AreEqual(5, result);

			// The same thing can be done like this, in a single lambda in .Returns()
			mock = new Mock<IVolume>();
			mock.Setup(x => x.Louder(It.IsAny<int>()))
				.Returns<int>(input =>
					{
						louderWasCalled = true;
						return input;
					});

			result = mock.Object.Louder(5);

			Assert.AreEqual(true, louderWasCalled);
			Assert.AreEqual(5, result);
		}
	}
}
