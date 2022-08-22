using Microsoft.VisualStudio.TestTools.UnitTesting;
using SvnToJira.Parameters;
using System.Collections.Generic;
using System.Linq;
using SvnToJira.Engine;
using Prometeia.AlmProTools.UnitTestHelpers;
using System;

namespace SvnToJiraTest
{
    [TestClass]
    public class BranchCheckerEngineTest
    {
        [TestMethod]

        /*
          checkedBranch = 5.30.0
          commitDiffList = 5.30.0
          expected = 5.30.0
          actual = 5.30.0
          EQUALS = true
        */
        public void Execute_Relevant_Branch_Return_Relevant_Branches()
        {
            #region Arrange

            var checkedBranches = new List<ReleasesBranchInfo>();

            checkedBranches.Add(
                new ReleasesBranchInfo()
                {
                    Path = "/branches/releases/5.30.0",
                    ReleaseName = "Ermas 5.30.0"
                }
            );

            var commitDiffList = new List<string>();
            commitDiffList.Add("/branches/releases/5.30.0");
            commitDiffList.Add("/branches/releases/5.30.0/AlmProSuite");
            commitDiffList.Add("/branches/releases/5.30.0/AlmProSuite/Source");
            commitDiffList.Add("/branches/releases/5.30.0/AlmProSuite/Source/ALMProSystem");
            commitDiffList.Add("/branches/releases/5.30.0/AlmProSuite/Source/ALMProSystem/Prometeia.ALMPro.EntityServices");
            commitDiffList.Add("/branches/releases/5.30.0/AlmProSuite/Source/ALMProSystem/Prometeia.ALMPro.EntityServices/DAL/SQL2005/Flow/FlowEntity_DALSQL2005.cs");


            var expected = new List<ReleasesBranchInfo>();
            expected.Add(
                new ReleasesBranchInfo()
                {
                    Path = "/branches/releases/5.30.0",
                    ReleaseName = "Ermas 5.30.0"
                }
            );

            var engine = new BranchCheckerEngine();

            #endregion

            #region Act

            var actual = engine.Execute(checkedBranches,commitDiffList);

            #endregion

            #region Assert

            AssertGeneric.AreEqual(expected, actual);

            #endregion

        }


        /*
          checkedBranch = 5.30.0
          commitDiffList = 5.30.0
          expected = 5.29.0
          actual = 5.30.0
          EQUALS = false
        */
        public void Execute_Relevant_Branch_Return_Relevant_Branches1()
           {
               #region Arrange

               var checkedBranches = new List<ReleasesBranchInfo>();

               checkedBranches.Add(
                   new ReleasesBranchInfo()
                   {
                       Path = "/branches/releases/5.30.0",
                       ReleaseName = "Ermas 5.30.0"
                   }
               );


               var commitDiffList = new List<string>();
               commitDiffList.Add("/branches/releases/5.30.0");
               commitDiffList.Add("/branches/releases/5.30.0/AlmProSuite");
               commitDiffList.Add("/branches/releases/5.30.0/AlmProSuite/Source");
               commitDiffList.Add("/branches/releases/5.30.0/AlmProSuite/Source/ALMProSystem");
               commitDiffList.Add("/branches/releases/5.30.0/AlmProSuite/Source/ALMProSystem/Prometeia.ALMPro.EntityServices");
               commitDiffList.Add("/branches/releases/5.30.0/AlmProSuite/Source/ALMProSystem/Prometeia.ALMPro.EntityServices/DAL/SQL2005/Flow/FlowEntity_DALSQL2005.cs");


               var expected = new List<ReleasesBranchInfo>();
               expected.Add(
                   new ReleasesBranchInfo()
                   {
                       Path = "/branches/releases/5.29.0",
                       ReleaseName = "Ermas 5.29.0"
                   }
               );

               var engine = new BranchCheckerEngine();

               #endregion

               #region Act

               var actual = engine.Execute(checkedBranches, commitDiffList);

               #endregion

               #region Assert

               AssertGeneric.AreEqual(expected, actual);

               #endregion

           }


        /*
          checkedBranch = 5.30.0
          commitDiffList = 5.29.0
          expected = 5.29.0
          actual = null
          EQUALS = false
        */
        public void Execute_Relevant_Branch_Return_Relevant_Branches2()
        {
            #region Arrange

            var checkedBranches = new List<ReleasesBranchInfo>();

            checkedBranches.Add(
                new ReleasesBranchInfo()
                {
                    Path = "/branches/releases/5.30.0",
                    ReleaseName = "Ermas 5.30.0"
                }
            );


            var commitDiffList = new List<string>();
            commitDiffList.Add("/branches/releases/5.29.0");
            commitDiffList.Add("/branches/releases/5.29.0/AlmProSuite");
            commitDiffList.Add("/branches/releases/5.29.0/AlmProSuite/Source");
            commitDiffList.Add("/branches/releases/5.29.0/AlmProSuite/Source/ALMProSystem");
            commitDiffList.Add("/branches/releases/5.29.0/AlmProSuite/Source/ALMProSystem/Prometeia.ALMPro.EntityServices");
            commitDiffList.Add("/branches/releases/5.29.0/AlmProSuite/Source/ALMProSystem/Prometeia.ALMPro.EntityServices/DAL/SQL2005/Flow/FlowEntity_DALSQL2005.cs");


            var expected = new List<ReleasesBranchInfo>();
            expected.Add(
                new ReleasesBranchInfo()
                {
                    Path = "/branches/releases/5.29.0",
                    ReleaseName = "Ermas 5.29.0"
                }
            );

            var engine = new BranchCheckerEngine();

            #endregion

            #region Act

            var actual = engine.Execute(checkedBranches, commitDiffList);

            #endregion

            #region Assert

            AssertGeneric.AreEqual(expected, actual);

            #endregion

        }


        /*
          checkedBranch = 5.30.0
          commitDiffList = 5.29.0
          expected = 5.30.0
          actual = null
          EQUALS = false
        */
        public void Execute_Relevant_Branch_Return_Relevant_Branches3()
        {
            #region Arrange

            var checkedBranches = new List<ReleasesBranchInfo>();

            checkedBranches.Add(
                new ReleasesBranchInfo()
                {
                    Path = "/branches/releases/5.30.0",
                    ReleaseName = "Ermas 5.30.0"
                }
            );


            var commitDiffList = new List<string>();
            commitDiffList.Add("/branches/releases/5.29.0");
            commitDiffList.Add("/branches/releases/5.29.0/AlmProSuite");
            commitDiffList.Add("/branches/releases/5.29.0/AlmProSuite/Source");
            commitDiffList.Add("/branches/releases/5.29.0/AlmProSuite/Source/ALMProSystem");
            commitDiffList.Add("/branches/releases/5.29.0/AlmProSuite/Source/ALMProSystem/Prometeia.ALMPro.EntityServices");
            commitDiffList.Add("/branches/releases/5.29.0/AlmProSuite/Source/ALMProSystem/Prometeia.ALMPro.EntityServices/DAL/SQL2005/Flow/FlowEntity_DALSQL2005.cs");


            var expected = new List<ReleasesBranchInfo>();
            expected.Add(
                new ReleasesBranchInfo()
                {
                    Path = "/branches/releases/5.29.0",
                    ReleaseName = "Ermas 5.29.0"
                }
            );

            var engine = new BranchCheckerEngine();

            #endregion

            #region Act

            var actual = engine.Execute(checkedBranches, commitDiffList);

            #endregion

            #region Assert

            AssertGeneric.AreEqual(expected, actual);

            #endregion

        }


        /*
          MULTIPLE CHECKEDBRANCH + CHECK SAME RELEASE FILE 
          checkedBranch = 5.30.0 - 5.29.0  --> anche invertiti
          commitDiffList = 5.29.0
          expected = 5.29.0
          actual = 5.29.0
          EQUALS = true
        */
        public void Execute_Relevant_Branch_Return_Relevant_Branches4()
        {
            #region Arrange

            var checkedBranches = new List<ReleasesBranchInfo>();

            checkedBranches.Add(
                new ReleasesBranchInfo()
                {
                    Path = "/branches/releases/5.30.0",
                    ReleaseName = "Ermas 5.30.0"
                }
            );
            checkedBranches.Add(
                new ReleasesBranchInfo()
                {
                    Path = "/branches/releases/5.29.0",
                    ReleaseName = "Ermas 5.29.0"
                }
            );


            var commitDiffList = new List<string>();
            commitDiffList.Add("/branches/releases/5.29.0");
            commitDiffList.Add("/branches/releases/5.29.0/AlmProSuite");
            commitDiffList.Add("/branches/releases/5.29.0/AlmProSuite/Source");
            commitDiffList.Add("/branches/releases/5.29.0/AlmProSuite/Source/ALMProSystem");
            commitDiffList.Add("/branches/releases/5.29.0/AlmProSuite/Source/ALMProSystem/Prometeia.ALMPro.EntityServices");
            commitDiffList.Add("/branches/releases/5.29.0/AlmProSuite/Source/ALMProSystem/Prometeia.ALMPro.EntityServices/DAL/SQL2005/Flow/FlowEntity_DALSQL2005.cs");

            var expected = new List<ReleasesBranchInfo>();
            expected.Add(
                new ReleasesBranchInfo()
                {
                    Path = "/branches/releases/5.29.0",
                    ReleaseName = "Ermas 5.29.0"
                }
            );

            var engine = new BranchCheckerEngine();

            #endregion

            #region Act
            //controlla che i file modificati ovvero quelli in commiDiffList siano tutti della stessa release
            string first=null;
            int count = 0;
            foreach(var i in commitDiffList)
            {
                if (count == 0)
                {
                    first = i.Substring(0,25);
                    count++;
                }
                else
                { 
                    if(first != i.Substring(0, 25))
                    {
                        throw new Exception("Modificati file di più release...");
                    }
                }
            }


            var actual = engine.Execute(checkedBranches, commitDiffList);

            #endregion

            #region Assert

            AssertGeneric.AreEqual(expected, actual);

            #endregion

        }

    }
}
