using MathExpressionEvalHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThomsonReutersMathExpressionEval
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {            
        }

        //Author - Vipin Agarwal (Vipin.Agarwal83@Gmail.com)
        private void btnEvaluate_Click(object sender, EventArgs e)
        {
            try
            {
                string strExpression = "";
                strExpression = txtInputExpression.Text; 
                
                MathExpressionEval expression = new MathExpressionEval(strExpression.Replace(" ", ""));  //remove empty spaces in the expression                             
                txtPostFixExpression.Text = string.Join(" ", expression.PostfixTokens);
                txtAnswer.Text = expression.Evaluate().ToString();
                 
                txtInputExpression.Focus();
            }
            catch (Exception Err)
            {
                txtPostFixExpression.Text = Err.Message;
                txtAnswer.Text = string.Empty;                
            }

        }
    }
}
