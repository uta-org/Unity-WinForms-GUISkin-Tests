using System;
using System.Drawing;
using System.Windows.Forms;
using UnityEngine;

using UnityWinForms.Examples;
using Screen = UnityEngine.Screen;

public class UnityFormExample : MonoBehaviour
{
    public static Material s_chartGradient;
    public Material ChartGradient;

    private void Start()
    {
        s_chartGradient = ChartGradient;

        var form = new FormExamples();
        form.Shown += Form_OnShown;

        form.Show();
    }

    private void Form_OnShown(object sender, EventArgs e)
    {
        var form = sender as Form;
        form.Location = new Point(Screen.width - form.Size.Width - 5, Screen.height - form.Size.Height - 5);
    }
}