using System.IO;
using System.Text;
using TCAProcess;

namespace Results
{
    internal class CSVExport : ITCAResultEvaluator
    {
        private readonly string fileFullPath;

        #region Constructors

        public CSVExport(string fileFullPath)
        {
            this.fileFullPath = fileFullPath;
        }

        #endregion

        #region Public methods

        public void Execute(TCAResultObj tcaObj)
        {
            ToCSV(tcaObj);
        }

        #endregion
        
        #region Private methods

        private void ToCSV(TCAResultObj tcaObj)
        {
            var csv = new StringBuilder();

            csv.AppendLine(string.Join(",", tcaObj.Titles.Values));

            foreach (var tcaRow in tcaObj.Values)
                csv.AppendLine(string.Join(",", tcaRow.Values));

            File.WriteAllText(fileFullPath, csv.ToString());
        }

        #endregion
    }
}
