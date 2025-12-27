using System;
using System.Drawing;
using System.Windows.Forms;

namespace Plugin.Compiler.UI
{
	internal partial class MessageCtrl : UserControl
	{
		private Timer _hideTimer;

		public enum StatusMessageType
		{
			Success = 0,
			Progress = 1,
			Failed = 2,
		}

		private static readonly Color[] StatusMessageColor = new Color[] { Color.LightCyan, Color.AntiqueWhite, Color.Pink, };

		public MessageCtrl()
		{
			this.InitializeComponent();
			this.Visible = false;

			lblMessage.SendToBack();

			this._hideTimer = new Timer()
			{
				Interval = 5000, // Auto-hide after 5 seconds
			};
			this._hideTimer.Tick += (s, e) => { this.Visible = false; this._hideTimer.Stop(); };
		}

		public void ShowMessage(StatusMessageType type, String message)
		{
			if(this.InvokeRequired)
			{
				this.BeginInvoke(new Action(() => this.ShowMessage(type, message)));
				return;
			}

			this._hideTimer.Stop();

			if(message == null)
				this.Visible = false;
			else
			{
				this.Visible = true;
				base.BackColor = MessageCtrl.StatusMessageColor[(Int32)type];
				lblMessage.Text = message;

				if(type == StatusMessageType.Success)
					this._hideTimer.Start();
			}
		}

		private void bnClose_MouseHover(Object sender, EventArgs e)
			=> bnClose.ImageIndex = 1;

		private void bnClose_MouseLeave(Object sender, EventArgs e)
			=> bnClose.ImageIndex = 0;

		private void bnClose_Click(Object sender, EventArgs e)
		{
			this.Visible = false;
			this._hideTimer.Stop();
		}
	}
}