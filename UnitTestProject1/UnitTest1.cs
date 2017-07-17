using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Poker.MainWindow;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestNothing()
        {
            var cards = new List<Card>() {
                new Card(CardDeck.Suit.Diamonds, 0),
                new Card(CardDeck.Suit.Clubs, 1),
                new Card(CardDeck.Suit.Clubs, 3),
                new Card(CardDeck.Suit.Clubs, 4),
                new Card(CardDeck.Suit.Clubs, 5)
            };
            var y = new PokerHand(cards);
            Assert.AreEqual(PokerHand.HandValues.Nothing, y.PokerValue());
        }
        [TestMethod]
        public void TestPair()
        {
            var cards = new List<Card>() {
                new Card(CardDeck.Suit.Diamonds, 4),
                new Card(CardDeck.Suit.Clubs, 1),
                new Card(CardDeck.Suit.Clubs, 3),
                new Card(CardDeck.Suit.Clubs, 4),
                new Card(CardDeck.Suit.Clubs, 5)
            };
            var y = new PokerHand(cards);
            Assert.AreEqual(PokerHand.HandValues.Pair, y.PokerValue());
        }
        [TestMethod]
        public void TestTwoPair()
        {
            var cards = new List<Card>() {
                new Card(CardDeck.Suit.Diamonds, 1),
                new Card(CardDeck.Suit.Clubs, 1),
                new Card(CardDeck.Suit.Clubs, 3),
                new Card(CardDeck.Suit.Spades, 3),
                new Card(CardDeck.Suit.Clubs, 5)
            };
            var y = new PokerHand(cards);
            Assert.AreEqual(PokerHand.HandValues.TwoPair, y.PokerValue());
        }
        [TestMethod]
        public void TestThreeOfAKind()
        {
            var cards = new List<Card>() {
                new Card(CardDeck.Suit.Diamonds, 6),
                new Card(CardDeck.Suit.Clubs, 1),
                new Card(CardDeck.Suit.Clubs, 3),
                new Card(CardDeck.Suit.Spades, 3),
                new Card(CardDeck.Suit.Diamonds, 3)
            };
            var y = new PokerHand(cards);
            Assert.AreEqual(PokerHand.HandValues.ThreeOfAKind, y.PokerValue());
        }
        [TestMethod]
        public void TestFourOfAKind()
        {
            var cards = new List<Card>() {
                new Card(CardDeck.Suit.Diamonds, 6),
                new Card(CardDeck.Suit.Hearts, 3),
                new Card(CardDeck.Suit.Clubs, 3),
                new Card(CardDeck.Suit.Spades, 3),
                new Card(CardDeck.Suit.Diamonds, 3)
            };
            var y = new PokerHand(cards);
            Assert.AreEqual(PokerHand.HandValues.FourOfAKind, y.PokerValue());
        }

        [TestMethod]
        public void TestFullHouse()
        {
            var cards = new List<Card>() {
                new Card(CardDeck.Suit.Diamonds, 1),
                new Card(CardDeck.Suit.Hearts, 1),
                new Card(CardDeck.Suit.Clubs, 3),
                new Card(CardDeck.Suit.Hearts, 3),
                new Card(CardDeck.Suit.Spades, 3)
            };
            var y = new PokerHand(cards);
            Assert.AreEqual(PokerHand.HandValues.FullHouse, y.PokerValue());
        }
        [TestMethod]
        public void TestFlush()
        {
            var cards = new List<Card>() {
                new Card(CardDeck.Suit.Clubs, 1),
                new Card(CardDeck.Suit.Clubs, 2),
                new Card(CardDeck.Suit.Clubs, 3),
                new Card(CardDeck.Suit.Clubs, 9),
                new Card(CardDeck.Suit.Clubs, 5)
            };
            var y = new PokerHand(cards);
            Assert.AreEqual(PokerHand.HandValues.Flush, y.PokerValue());
        }

        [TestMethod]
        public void TestStraight()
        {
            var cards = new List<Card>() {
                new Card(CardDeck.Suit.Diamonds, 3),
                new Card(CardDeck.Suit.Clubs, 4),
                new Card(CardDeck.Suit.Clubs, 5),
                new Card(CardDeck.Suit.Clubs, 6),
                new Card(CardDeck.Suit.Clubs, 7)
            };
            var y = new PokerHand(cards);
            Assert.AreEqual(PokerHand.HandValues.Straight, y.PokerValue());
        }
        [TestMethod]
        public void TestStraightAceLow()
        {
            var cards = new List<Card>() {
                new Card(CardDeck.Suit.Diamonds, 12),
                new Card(CardDeck.Suit.Clubs, 0),
                new Card(CardDeck.Suit.Clubs, 1),
                new Card(CardDeck.Suit.Clubs, 2),
                new Card(CardDeck.Suit.Clubs, 3)
            };
            var y = new PokerHand(cards);
            Assert.AreEqual(PokerHand.HandValues.Straight, y.PokerValue());

            cards = new List<Card>() {
                new Card(CardDeck.Suit.Diamonds, 12),
                new Card(CardDeck.Suit.Clubs, 0),
                new Card(CardDeck.Suit.Clubs, 1),
                new Card(CardDeck.Suit.Clubs, 2),
                new Card(CardDeck.Suit.Clubs, 4)
            };
            y = new PokerHand(cards);
            Assert.AreEqual(PokerHand.HandValues.Nothing, y.PokerValue());
        }

        [TestMethod]
        public void TestStraightFlush()
        {
            var cards = new List<Card>() {
                new Card(CardDeck.Suit.Clubs, 1),
                new Card(CardDeck.Suit.Clubs, 2),
                new Card(CardDeck.Suit.Clubs, 3),
                new Card(CardDeck.Suit.Clubs, 4),
                new Card(CardDeck.Suit.Clubs, 5)
            };
            var y = new PokerHand(cards);
            Assert.AreEqual(PokerHand.HandValues.StraightFlush, y.PokerValue());
        }

        [TestMethod]
        public void TestRoyalFlush()
        {
            var cards = new List<Card>() {
                new Card(CardDeck.Suit.Clubs, 8),
                new Card(CardDeck.Suit.Clubs, 9),
                new Card(CardDeck.Suit.Clubs, 10),
                new Card(CardDeck.Suit.Clubs,11),
                new Card(CardDeck.Suit.Clubs, 12)
            };
            var y = new PokerHand(cards);
            Assert.AreEqual(PokerHand.HandValues.RoyalFlush, y.PokerValue());
        }
    }
}
