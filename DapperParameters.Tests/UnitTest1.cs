using System;
using System.Collections.Generic;
using Dapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RandomTestValues;

namespace DapperParameters.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private DynamicParameters _parameters;

        [TestInitialize]
        public void Setup()
        {
            _parameters = new DynamicParameters();
        }
        
        [TestMethod]
        public void AddInt()
        {
            int intValue = RandomValue.Int();
            _parameters.AddInt("test", intValue);

            _parameters.ParameterNames.Should().Contain("test");
            
            var value = _parameters.Get<int>("test");
            value.ShouldBeEquivalentTo(intValue);
        }

        [TestMethod]
        public void AddString()
        {
            var stringValue = RandomValue.String();
            var name = RandomValue.String();
            _parameters.AddString(name, stringValue);

            _parameters.ParameterNames.Should().Contain(name);

            var value = _parameters.Get<string>(name);
            value.ShouldBeEquivalentTo(stringValue);
        }

        [TestMethod]
        public void Table()
        {
            // Act
            _parameters.AddTable("test", "testType", RandomValue.List<IntListType>());

            // Assert
            var x = _parameters.Get<SqlMapper.ICustomQueryParameter>("test");
            x.Should().NotBeNull();
        }
    }

    public class IntListType
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
    }
}
