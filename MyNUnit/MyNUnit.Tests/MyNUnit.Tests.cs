// <copyright file="MyNUnit.Tests.cs" company="NematMusaev">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace MyNUnit.Tests;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;

/// <summary>
/// class of tests.
/// </summary>
    public class SampleTests
    {
        /// <summary>
        /// flag of before classes methodes.
        /// </summary>
        public static bool BeforeClassFlag = false;

        /// <summary>
        /// flag of after classes methodes.
        /// </summary>
        public static bool AfterClassFlag = false;

        /// <summary>
        /// before methodes are called.
        /// </summary>
        [Tests.BeforeClass]
        public static void BeforeClassTests()
        {
            BeforeClassFlag = true;
        }

        /// <summary>
        /// after methodes are called.
        /// </summary>
        [Tests.AfterClass]
        public static void AfterClassTests()
        {
            AfterClassFlag = true;
        }

        /// <summary>
        /// before tests.
        /// </summary>
        [Tests.Before]
        public void BeforeTest()
        {
        }

        /// <summary>
        /// after methodes are called.
        /// </summary>
        [Tests.After]
        public void AfterTest()
        {
        }

        /// <summary>
        /// main test.
        /// </summary>
        [Tests.Test]
        public void Test1()
        {
            Assert.IsTrue(BeforeClassFlag);
            Assert.IsFalse(AfterClassFlag);
        }

        /// <summary>
        /// running of our NUnit.
        /// </summary>
        [Test]
        public void RunTests()
        {
            Tests.Run(AppDomain.CurrentDomain.BaseDirectory);
        }
    }
