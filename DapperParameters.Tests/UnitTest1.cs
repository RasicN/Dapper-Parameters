using System;
using Dapper;
using DapperParameters.Attributes;
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
        public void Table()
        {
            // Act
            _parameters.AddTable("test", "testType", RandomValue.List<IntListType>());

            // Assert
            var x = _parameters.Get<SqlMapper.ICustomQueryParameter>("test");
            x.Should().NotBeNull();
        }

        [TestMethod]
        public void TableWithNulls()
        {
            // Act
            _parameters.AddTable("test", "testType", RandomValue.List<IntListTypeWithNullables>());

            // Assert
            var x = _parameters.Get<SqlMapper.ICustomQueryParameter>("test");
            x.Should().NotBeNull();
        }

        [TestMethod]
        public void TableWithIgnoredAttribute()
        {
            // Act
            _parameters.AddTable("test", "testType", RandomValue.List<IgnoredIntListIdype>());

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

    public class IntListTypeWithNullables
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
    }

    public class IgnoredIntListIdype
    {
        [IgnoreProperty]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
    }
}
