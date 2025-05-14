using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Puzzle8
{
    public partial class Form1 : Form
    {
        private Button[] botonesEstadoInicial;
        private Button[] botonesEstadoObjetivo;
        private int movimientos = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InicializarTablero();
        }

        private void InicializarTablero()
        {
            botonesEstadoInicial = new Button[9];
            botonesEstadoObjetivo = new Button[9];

            // Crear botones del estado inicial
            for (int i = 0; i < 9; i++)
            {
                Button btn = new Button();
                btn.Width = 50;
                btn.Height = 50;
                btn.Left = 20 + (i % 3) * 55;
                btn.Top = 20 + (i / 3) * 55;
                btn.Font = new System.Drawing.Font("Arial", 14);
                btn.Click += new EventHandler(Boton_Click);
                this.Controls.Add(btn);
                botonesEstadoInicial[i] = btn;
            }

            // Crear botones del estado objetivo
            int[] estadoMeta = { 1, 2, 3, 8, 0, 4, 7, 6, 5 };
            for (int i = 0; i < 9; i++)
            {
                Button btn = new Button();
                btn.Width = 50;
                btn.Height = 50;
                btn.Left = 220 + (i % 3) * 55;
                btn.Top = 20 + (i / 3) * 55;
                btn.Font = new System.Drawing.Font("Arial", 14);
                btn.Text = estadoMeta[i] == 0 ? "" : estadoMeta[i].ToString();
                btn.Enabled = false;
                this.Controls.Add(btn);
                botonesEstadoObjetivo[i] = btn;
            }

            // Botón para cargar estado inicial manual
            Button btnEstadoInicial = new Button();
            btnEstadoInicial.Text = "Cargar Estado Inicial";
            btnEstadoInicial.Top = 200;
            btnEstadoInicial.Left = 20;
            btnEstadoInicial.Click += new EventHandler(CargarEstadoInicial);
            this.Controls.Add(btnEstadoInicial);

            // Label para contador de movimientos
            Label lbl = new Label();
            lbl.Name = "lblMovimientos";
            lbl.Text = "Movimientos: 0";
            lbl.Top = 200;
            lbl.Left = 200;
            lbl.Width = 200;
            this.Controls.Add(lbl);

            // Botón para aleatorio
            Button btnAleatorio = new Button();
            btnAleatorio.Text = "Aleatorio";
            btnAleatorio.Top = 240;
            btnAleatorio.Left = 20;
            btnAleatorio.Click += new EventHandler(CargarEstadoAleatorio);
            this.Controls.Add(btnAleatorio);

            // Botón para resolver automáticamente
            Button btnResolver = new Button();
            btnResolver.Text = "Resolver Automáticamente";
            btnResolver.Top = 240;
            btnResolver.Left = 150;
            btnResolver.Click += new EventHandler(ResolverAutomaticamente);
            this.Controls.Add(btnResolver);
        }

        private void CargarEstadoInicial(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Ingrese los números del 0 al 8 separados por coma (ej: 1,2,3,4,5,6,7,8,0):",
                "Estado Inicial");

            string[] valores = input.Split(',');
            if (valores.Length != 9)
            {
                MessageBox.Show("Debe ingresar exactamente 9 valores.");
                return;
            }

            HashSet<int> numeros = new HashSet<int>();
            for (int i = 0; i < 9; i++)
            {
                if (!int.TryParse(valores[i], out int val) || val < 0 || val > 8 || !numeros.Add(val))
                {
                    MessageBox.Show("Los valores deben ser únicos y estar entre 0 y 8.");
                    return;
                }
                botonesEstadoInicial[i].Text = val == 0 ? "" : val.ToString();
            }

            movimientos = 0;
            ActualizarContador();
        }

        private void CargarEstadoAleatorio(object sender, EventArgs e)
        {
            Random rand = new Random();
            List<int> valores = Enumerable.Range(0, 9).OrderBy(x => rand.Next()).ToList();

            for (int i = 0; i < 9; i++)
            {
                botonesEstadoInicial[i].Text = valores[i] == 0 ? "" : valores[i].ToString();
            }

            movimientos = 0;
            ActualizarContador();
        }

        private void Boton_Click(object sender, EventArgs e)
        {
            Button btnClicado = (Button)sender;
            int idxClicado = Array.IndexOf(botonesEstadoInicial, btnClicado);
            int idxVacio = Array.FindIndex(botonesEstadoInicial, b => b.Text == "");

            if (EsAdyacente(idxClicado, idxVacio))
            {
                Intercambiar(botonesEstadoInicial[idxClicado], botonesEstadoInicial[idxVacio]);
                movimientos++;
                ActualizarContador();
                VerificarObjetivo();
            }
        }

        private bool EsAdyacente(int a, int b)
        {
            int filaA = a / 3, colA = a % 3;
            int filaB = b / 3, colB = b % 3;

            return (Math.Abs(filaA - filaB) == 1 && colA == colB) ||
                   (Math.Abs(colA - colB) == 1 && filaA == filaB);
        }

        private void Intercambiar(Button b1, Button b2)
        {
            string temp = b1.Text;
            b1.Text = b2.Text;
            b2.Text = temp;
        }

        private void VerificarObjetivo()
        {
            for (int i = 0; i < 9; i++)
            {
                if (botonesEstadoInicial[i].Text != botonesEstadoObjetivo[i].Text)
                    return;
            }

            MessageBox.Show("¡Felicidades! Has resuelto el puzzle en " + movimientos + " movimientos.");
        }

        private void ActualizarContador()
        {
            Label lbl = this.Controls.Find("lblMovimientos", true).FirstOrDefault() as Label;
            if (lbl != null)
                lbl.Text = "Movimientos: " + movimientos;
        }

        private IEnumerable<string> GenerarVecinos(string estado)
        {
            string[] val = estado.Split(',');
            int[] puzzle = val.Select(int.Parse).ToArray();
            int idxVacio = Array.IndexOf(puzzle, 0);
            int fila = idxVacio / 3, col = idxVacio % 3;

            int[] dFilas = { -1, 1, 0, 0 };
            int[] dCols = { 0, 0, -1, 1 };

            for (int d = 0; d < 4; d++)
            {
                int nuevaFila = fila + dFilas[d];
                int nuevaCol = col + dCols[d];

                if (nuevaFila >= 0 && nuevaFila < 3 && nuevaCol >= 0 && nuevaCol < 3)
                {
                    int nuevoIdx = nuevaFila * 3 + nuevaCol;
                    int[] nuevoPuzzle = (int[])puzzle.Clone();
                    (nuevoPuzzle[idxVacio], nuevoPuzzle[nuevoIdx]) = (nuevoPuzzle[nuevoIdx], nuevoPuzzle[idxVacio]);
                    yield return string.Join(",", nuevoPuzzle);
                }
            }
        }

        private async void ResolverAutomaticamente(object sender, EventArgs e)
        {
            string inicio = string.Join(",", botonesEstadoInicial.Select(b => b.Text == "" ? "0" : b.Text));
            string objetivo = "1,2,3,8,0,4,7,6,5"; // 1, 2, 3, 8, 0, 4, 7, 6, 5

            if (inicio == objetivo)
            {
                MessageBox.Show("Ya estás en el estado objetivo.");
                return;
            }

            Queue<string> cola = new Queue<string>();
            Dictionary<string, string> padre = new Dictionary<string, string>();
            cola.Enqueue(inicio);
            padre[inicio] = null;

            while (cola.Count > 0)
            {
                string actual = cola.Dequeue();

                if (actual == objetivo)
                    break;

                foreach (string vecino in GenerarVecinos(actual))
                {
                    if (!padre.ContainsKey(vecino))
                    {
                        cola.Enqueue(vecino);
                        padre[vecino] = actual;
                    }
                }
            }

            if (!padre.ContainsKey(objetivo))
            {
                MessageBox.Show("No se encontró solución.");
                return;
            }

            // Reconstruir camino
            List<string> camino = new List<string>();
            string paso = objetivo;
            while (paso != null)
            {
                camino.Add(paso);
                paso = padre[paso];
            }

            camino.Reverse();

            // Ejecutar animación paso a paso
            foreach (string estado in camino)
            {
                string[] valores = estado.Split(',');
                for (int i = 0; i < 9; i++)
                {
                    botonesEstadoInicial[i].Text = valores[i] == "0" ? "" : valores[i];
                }
                await Task.Delay(400);
            }

            movimientos += camino.Count - 1;
            ActualizarContador();
            MessageBox.Show("Puzzle resuelto automáticamente.");
        }
    }
}
