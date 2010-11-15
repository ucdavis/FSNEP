using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSNEP.Core.Abstractions;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;


namespace FSNEP.Tests.Core.GeneralTests
{
    [TestClass]
    public class SignatureFactoryTests
    {
        readonly ISignatureFactory _signatureFactory = new SignatureFactory();
        private readonly IRepository _repository = MockRepository.GenerateStub<IRepository>();
        private readonly IRepository<CostShareEntry> _costShareEntryRepository =
            MockRepository.GenerateStub<IRepository<CostShareEntry>>();
        private readonly IRepository<TimeRecordEntry> _timeRecordEntryRepository =
            MockRepository.GenerateStub<IRepository<TimeRecordEntry>>();

        private List<CostShare> _costShares;
        private List<CostShareEntry> _costShareEntries;

        private List<TimeRecord> _timeRecords;
        private List<TimeRecordEntry> _timeRecordEntries;

        #region Init

        [TestInitialize]
        public void Setup()
        {
            _repository.Expect(a => a.OfType<CostShareEntry>()).Return(_costShareEntryRepository).Repeat.Any();
            _repository.Expect(a => a.OfType<TimeRecordEntry>()).Return(_timeRecordEntryRepository).Repeat.Any();

            _costShares = new List<CostShare>();
            _costShareEntries = new List<CostShareEntry>();
            FakeCostShareEntries();

            _timeRecords = new List<TimeRecord>();
            _timeRecordEntries = new List<TimeRecordEntry>();
            FakeTimeRecordEntries();
        }

        /// <summary>
        /// Fakes the time record entries.
        /// </summary>
        private void FakeTimeRecordEntries()
        {
            int counter = 0;
            //Create 3 TimeRecord Records (and Entries) with the same data
            for (int i = 0; i < 3; i++)
            {
                _timeRecords.Add(CreateValidEntities.TimeRecord(null));
                _timeRecords[i].User = CreateValidEntities.User(null);
                _timeRecords[i].SetIdTo(i + 1);
                for (int j = 0; j < 3; j++)
                {
                    counter++;
                    _timeRecordEntries.Add(CreateValidEntities.TimeRecordEntry(null));
                    _timeRecordEntries[counter - 1].SetIdTo(counter);
                    _timeRecordEntries[counter - 1].Record = _timeRecords[i];
                }
            }

            //Create 3 TimeRecord Records (and Entries) with DIFFERENT data
            for (int i = 3; i < 6; i++)
            {
                _timeRecords.Add(CreateValidEntities.TimeRecord(i + 1));
                _timeRecords[i].User = CreateValidEntities.User(i + 1);
                _timeRecords[i].User.UserName = "UserName" + (i + 1);
                _timeRecords[i].SetIdTo(i + 1);
                for (int j = 0; j < 3; j++)
                {
                    counter++;
                    _timeRecordEntries.Add(CreateValidEntities.TimeRecordEntry(counter));
                    _timeRecordEntries[counter - 1].SetIdTo(counter);
                    _timeRecordEntries[counter - 1].Record = _timeRecords[i];
                }
            }

            _timeRecordEntryRepository.Expect(a => a.Queryable).Return(_timeRecordEntries.AsQueryable()).Repeat.Any();
        }

        /// <summary>
        /// Fakes the cost share entries.
        /// </summary>
        private void FakeCostShareEntries()
        {
            int counter = 0;
            //Create 3 CostShare Records (and Entries) with the same data
            for (int i = 0; i < 3; i++)
            {
                _costShares.Add(CreateValidEntities.CostShare(null));
                _costShares[i].User = CreateValidEntities.User(null);
                _costShares[i].SetIdTo(i + 1);
                for (int j = 0; j < 3; j++)
                {
                    counter++;
                    _costShareEntries.Add(CreateValidEntities.CostShareEntry(null));
                    _costShareEntries[counter - 1].SetIdTo(counter);
                    _costShareEntries[counter - 1].Record = _costShares[i];
                }
            }

            //Create 3 CostShare Records (and Entries) with DIFFERENT data
            for (int i = 3; i < 6; i++)
            {
                _costShares.Add(CreateValidEntities.CostShare(i + 1));
                _costShares[i].User = CreateValidEntities.User(i + 1);
                _costShares[i].User.UserName = "UserName" + (i + 1);
                _costShares[i].SetIdTo(i + 1);
                for (int j = 0; j < 3; j++)
                {
                    counter++;
                    _costShareEntries.Add(CreateValidEntities.CostShareEntry(counter));
                    _costShareEntries[counter - 1].SetIdTo(counter);
                    _costShareEntries[counter - 1].Record = _costShares[i];
                }
            }            
            _costShareEntryRepository.Expect(a => a.Queryable).Return(_costShareEntries.AsQueryable()).Repeat.Any();

        }

        #endregion Init

        #region CostShare Signature Tests


        /// <summary>
        /// Tests the cost share signature is the same for the different records that have the SAME data.
        /// The first three CostShare Records have the same data
        /// </summary>
        [TestMethod]
        public void TestCostShareSignatureIsTheSameForTheSameData()
        {
            var digitalSignature1 = _signatureFactory.CreateSignature(_costShares[0], _repository);
            var digitalSignature2 = _signatureFactory.CreateSignature(_costShares[1], _repository);

            var digitalSignatureString1 = new StringBuilder();
            var digitalSignatureString2 = new StringBuilder();
            foreach (var b in digitalSignature1)
            {
                digitalSignatureString1.Append(b);
            }
            foreach (var b in digitalSignature2)
            {
                digitalSignatureString2.Append(b);
            }

            Assert.IsNotNull(digitalSignature1);
            Assert.IsNotNull(digitalSignature2);
            Assert.AreEqual("2431556184240180209701646282442481421152318140107771333386124221242152181555373225",
                digitalSignatureString1.ToString());
            Assert.AreEqual(digitalSignatureString1.ToString(), digitalSignatureString2.ToString());
        }


        /// <summary>
        /// Tests the cost share signature is different for different data.
        /// the last three CostShare Records have different data
        /// </summary>
        [TestMethod]
        public void TestCostShareSignatureIsDifferentForDifferentData()
        {
            var digitalSignature1 = _signatureFactory.CreateSignature(_costShares[3], _repository);
            var digitalSignature2 = _signatureFactory.CreateSignature(_costShares[4], _repository);

            var digitalSignatureString1 = new StringBuilder();
            var digitalSignatureString2 = new StringBuilder();
            foreach (var b in digitalSignature1)
            {
                digitalSignatureString1.Append(b);
            }
            foreach (var b in digitalSignature2)
            {
                digitalSignatureString2.Append(b);
            }

            Assert.IsNotNull(digitalSignature1);
            Assert.AreEqual("8717830242232646717123224220218918188721542153015669131454102481024814613313192109",
                digitalSignatureString1.ToString());
            Assert.AreEqual("413987616710273372625181881878415913411633181193541793525011313481373975109217",
                            digitalSignatureString2.ToString());

            Assert.IsNotNull(digitalSignature2);
            Assert.AreNotEqual(digitalSignatureString1.ToString(), digitalSignatureString2.ToString());
        }

        /// <summary>
        /// Tests the cost share signature is different when comment is changed.
        /// </summary>
        [TestMethod]
        public void TestCostShareSignatureIsDifferentWhenCommentIsChanged()
        {
            var digitalSignature1 = _signatureFactory.CreateSignature(_costShares[0], _repository);

            _costShareEntries[1].Comment = "Updated";
            var digitalSignature2 = _signatureFactory.CreateSignature(_costShares[0], _repository);

            var digitalSignatureString1 = new StringBuilder();
            var digitalSignatureString2 = new StringBuilder();
            foreach (var b in digitalSignature1)
            {
                digitalSignatureString1.Append(b);
            }
            foreach (var b in digitalSignature2)
            {
                digitalSignatureString2.Append(b);
            }

            Assert.IsNotNull(digitalSignature1);
            Assert.IsNotNull(digitalSignature2);
            Assert.AreEqual("2431556184240180209701646282442481421152318140107771333386124221242152181555373225",
                digitalSignatureString1.ToString());
            Assert.AreEqual("19120836924686189185210250167592360122154187238255177149182223171182124798140716637",
                digitalSignatureString2.ToString());
            Assert.AreNotEqual(digitalSignatureString1.ToString(), digitalSignatureString2.ToString());
        }


        #endregion CostShare Signature Tests

        #region TimeRecord Signature Tests

        /// <summary>
        /// Tests the time record signature is the same for the same data.
        /// </summary>
        [TestMethod]
        public void TestTimeRecordSignatureIsTheSameForTheSameData()
        {
            var digitalSignature1 = _signatureFactory.CreateSignature(_timeRecords[0], _repository);
            var digitalSignature2 = _signatureFactory.CreateSignature(_timeRecords[1], _repository);

            var digitalSignatureString1 = new StringBuilder();
            var digitalSignatureString2 = new StringBuilder();
            foreach (var b in digitalSignature1)
            {
                digitalSignatureString1.Append(b);
            }
            foreach (var b in digitalSignature2)
            {
                digitalSignatureString2.Append(b);
            }

            Assert.IsNotNull(digitalSignature1);
            Assert.IsNotNull(digitalSignature2);
            Assert.AreEqual("191701971952492181116014625019653238248695116381742451886667202001012384128164",
                digitalSignatureString1.ToString());
            Assert.AreEqual(digitalSignatureString1.ToString(), digitalSignatureString2.ToString());
        }

        /// <summary>
        /// Tests the time record signature is different for different data.
        /// </summary>
        [TestMethod]
        public void TestTimeRecordSignatureIsDifferentForDifferentData()
        {
            var digitalSignature1 = _signatureFactory.CreateSignature(_timeRecords[3], _repository);
            var digitalSignature2 = _signatureFactory.CreateSignature(_timeRecords[4], _repository);

            var digitalSignatureString1 = new StringBuilder();
            var digitalSignatureString2 = new StringBuilder();
            foreach (var b in digitalSignature1)
            {
                digitalSignatureString1.Append(b);
            }
            foreach (var b in digitalSignature2)
            {
                digitalSignatureString2.Append(b);
            }

            Assert.IsNotNull(digitalSignature1);
            Assert.AreEqual("17321017113125236189926719522410513886117835333193811626210370742611823923146155",
                digitalSignatureString1.ToString());
            Assert.AreEqual("191244167123246437717315314671581523922449149237165211133190136135291578731250242220165",
                            digitalSignatureString2.ToString());

            Assert.IsNotNull(digitalSignature2);
            Assert.AreNotEqual(digitalSignatureString1.ToString(), digitalSignatureString2.ToString());
        }

        /// <summary>
        /// Tests the time record signature is different when comment is changed.
        /// </summary>
        [TestMethod]
        public void TestTimeRecordSignatureIsDifferentWhenCommentIsChanged()
        {
            var digitalSignature1 = _signatureFactory.CreateSignature(_timeRecords[0], _repository);

            _timeRecordEntries[1].Comment = "Updated";
            var digitalSignature2 = _signatureFactory.CreateSignature(_timeRecords[0], _repository);

            var digitalSignatureString1 = new StringBuilder();
            var digitalSignatureString2 = new StringBuilder();
            foreach (var b in digitalSignature1)
            {
                digitalSignatureString1.Append(b);
            }
            foreach (var b in digitalSignature2)
            {
                digitalSignatureString2.Append(b);
            }

            Assert.IsNotNull(digitalSignature1);
            Assert.IsNotNull(digitalSignature2);
            Assert.AreEqual("191701971952492181116014625019653238248695116381742451886667202001012384128164",
                digitalSignatureString1.ToString());
            Assert.AreEqual("6818938332316116012042422491131488111620322144113911817796241247981701933533168",
                digitalSignatureString2.ToString());
            Assert.AreNotEqual(digitalSignatureString1.ToString(), digitalSignatureString2.ToString());
        }

        #endregion TimeRecord Signature Tests

           
    }
}
