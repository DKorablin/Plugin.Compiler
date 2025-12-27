using System;

namespace Plugin.Compiler.UI.Dto
{
	internal class ErrorInfoItem
	{
		public String ErrorNumber { get; set; }

		public Boolean IsWarning { get; set; }

		public Int32 Line { get; set; }

		public String ErrorText { get; set; }

		public String Tag { get; set; }

		public ErrorInfoItem(String errorNumber, Boolean isWarning, Int32 line, String errorText)
		{
			this.ErrorNumber = errorNumber;
			this.IsWarning = isWarning;
			this.Line = line;
			this.ErrorText = errorText;
		}

		public ErrorInfoItem(String referenceAssembly, String errorText)
		{
			this.ErrorNumber = "REF001";
			this.IsWarning = false;
			this.Line = 0;
			this.ErrorText = $"Cannot load assembly reference {referenceAssembly}: {ErrorText}";
			this.Tag = referenceAssembly;
		}
	}
}