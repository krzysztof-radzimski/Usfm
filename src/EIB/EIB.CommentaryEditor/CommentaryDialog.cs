﻿using EIB.CommentaryEditor.Db.Model;
using System;
using System.Windows.Forms;

namespace EIB.CommentaryEditor {
    public partial class CommentaryDialog : DevExpress.XtraEditors.XtraForm {
        public Commentary Commentary { get; }

        private CommentaryDialog() {
            InitializeComponent();
            this.IconOptions.SvgImage = Properties.Resources.logo_eib;
            this.IconOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.None;
        }

        public CommentaryDialog(Commentary commentary) : this() {
            this.Commentary = commentary;
            LoadData();
        }

        private void LoadData() {
            txtInformation.RtfText = Commentary.Information;
            txtTitle.Text = Commentary.Title;
            txtAbbreviation.Text = Commentary.Abbreviation;
        }

        private void btnApply_Click(object sender, EventArgs e) {
            Commentary.Information = txtInformation.RtfText;
            Commentary.Title = txtTitle.Text;
            Commentary.Abbreviation = txtAbbreviation.Text;
            DialogResult = DialogResult.OK;
        }
    }
}