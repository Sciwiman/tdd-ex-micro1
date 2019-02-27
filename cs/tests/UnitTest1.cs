using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;

namespace TddExMicrotest.Tests
{
	public class FakeTheInternet : ITheInternet
	{
		private readonly string _result;

		public FakeTheInternet(string result)
		{
			_result = result;
		}

		public string Get(string url, params object[] parameters)
		{
			return "[{ word: \"" + _result + "\" }]";
		}
	}

	public class FakeBrokenTheInternet : ITheInternet
	{
		private readonly Exception _exception;

		public FakeBrokenTheInternet(Exception exception)
		{
			_exception = exception;
		}

		public string Get(string url, params object[] parameters)
		{
			throw _exception;
		}
	}

	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void SimpleWordOfCorrectLength()
		{
			// Arrange
			var theInternet = new FakeTheInternet("hello");

			// Act
			var x = Hangman.FromInternet(5, theInternet);

			// Assert
			Assert.AreEqual(5, x.NumberLetters);
		}

		[TestMethod]
		public void HandlesEmptyStringFromTheInternet()
		{
			// Arrange
			var theInternet = new FakeTheInternet(string.Empty);

			// Act
			var x = Hangman.FromInternet(5, theInternet);

			// Assert
			Assert.AreEqual(0, x.NumberLetters);
		}

		[TestMethod]
		[ExpectedException(typeof(WebException))]
		public void HandlesConnectFailureWebException()
		{
			// Arrange
			var theInternet = new FakeBrokenTheInternet(new WebException("ConnectFailure", WebExceptionStatus.ConnectFailure));

			// Act
			var hangman = Hangman.FromInternet(5, theInternet);
		}

		[TestMethod]
		public void MoqInternet()
		{
			// Arrange
			var mock = new Mock<ITheInternet>();
			mock.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<int>()))
				.Returns("[{ word: \"hello\" }]");

			// Act
			var hangman = Hangman.FromInternet(5, mock.Object);

			// Assert
			Assert.AreEqual(5, hangman.NumberLetters);
		}
	}
}