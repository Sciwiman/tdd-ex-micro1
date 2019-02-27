using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace TddExMicrotest
{
	public enum GameStatus { NotStarted, Guessing, Winner, Hanged };

	public enum LetterStatus { NotGuessed, GuessedCorrect, GuessedIncorrect };

	public class Letter
	{
		public char Value;
		public LetterStatus Status;

		public Letter(char value, LetterStatus status)
		{
			Value = value;
			Status = status;
		}
	}

	public class Hangman
	{
		private static Random R = new Random();
		private const int DEATH_LIMIT = 11;
		private Letter[] _letters = Enumerable.Range('A', 26).Select(x => new Letter((char)x, LetterStatus.NotGuessed)).ToArray();

		public readonly string _word;

		public Hangman(string word)
		{
			Console.WriteLine("Hangman started with word: " + word);
			_word = word;
			Status = GameStatus.NotStarted;
		}

		public static Hangman FromInternet(int length, ITheInternet theInternet)
		{
			Console.WriteLine("Starting hangman with a word of length " + length);
			var content = theInternet.Get(GameUrls.WORDS, length);
			var words = JArray.Parse(content);
			var index = R.Next(0, words.Count);
			var word = words[index].Value<string>("word");
			return new Hangman(word);
		}

		public GameStatus Status { get; private set; }

		public int NumberLetters => this._word.Length;

		public int NumberWrongGuesses
		{
			get
			{
				return _letters.Count(x => x.Status == LetterStatus.GuessedIncorrect);
			}
		}

		public int NumberRightGuesses
		{
			get
			{
				var letters = _word.ToCharArray();
				var letterStatuses = letters.Select(i => _letters[(i - 'a')]);
				return letterStatuses.Count(x => x.Status == LetterStatus.GuessedCorrect);
			}
		}

		public bool Guess(char letter)
		{
			Console.WriteLine("Guessing " + letter);
			if (Status != GameStatus.NotStarted && Status != GameStatus.Guessing)
			{
				throw new ApplicationException("Game not in state that can accept a guess.");
			}
			Status = GameStatus.Guessing;
			var isCorrect = _word.Contains(letter);
			_letters[(letter - 'a')].Status = (isCorrect ? LetterStatus.GuessedCorrect : LetterStatus.GuessedIncorrect);
			if (NumberWrongGuesses >= DEATH_LIMIT)
			{
				Console.WriteLine("Hanged!");
				Status = GameStatus.Hanged;
			}
			if (NumberRightGuesses >= NumberLetters)
			{
				Console.WriteLine("Winner!");
				Status = GameStatus.Winner;
			}
			Console.WriteLine("Letter is correct? " + isCorrect);
			return isCorrect;
		}
	}
}