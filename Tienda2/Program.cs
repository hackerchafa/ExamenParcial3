using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

class Producto
{
    public int codigoProducto { get; set; }
    public string nombreProducto { get; set; }
    public int cantidadStock { get; set; }
    public double precioProducto { get; set; }

    public Producto(int codigoProducto, string nombreProducto, int cantidadStock, double precioProducto)
    {
        this.codigoProducto = codigoProducto;
        this.nombreProducto = nombreProducto;
        this.cantidadStock = cantidadStock;
        this.precioProducto = precioProducto;
    }
}

class Registro
{
    private string _rutaArchivo;

    public Registro(string rutaArchivo)
    {
        this._rutaArchivo = rutaArchivo;
    }

    public List<Producto> ObtenerProductos()
    {
        if (File.Exists(_rutaArchivo))
        {
            string jsonString = File.ReadAllText(_rutaArchivo);
            if (!string.IsNullOrEmpty(jsonString))
            {
                return JsonSerializer.Deserialize<List<Producto>>(jsonString);
            }
        }
        return new List<Producto>();
    }

    public void GuardarProducto(Producto producto)
    {
        List<Producto> productos = ObtenerProductos();
        productos.Add(producto);

        string jsonNuevo = JsonSerializer.Serialize(productos, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_rutaArchivo, jsonNuevo);
    }

    public void ActualizarProductos(List<Producto> productos)
    {
        string jsonNuevo = JsonSerializer.Serialize(productos, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_rutaArchivo, jsonNuevo);
    }

    public void MostrarInfo()
    {
        List<Producto> productos = ObtenerProductos();
        if (productos.Count > 0)
        {
            foreach (var producto in productos)
            {
                Console.WriteLine($"\nCodigo: {producto.codigoProducto}, Producto: {producto.nombreProducto}, Cantidad en Stock: {producto.cantidadStock}, Precio: {producto.precioProducto}");
            }
        }
        else
        {
            Console.WriteLine("El archivo está vacío.");
        }
    }
}

class Compra
{
    public Producto ComprarProducto(int codigoProducto, int cantidad, Registro registro)
    {
        List<Producto> productos = registro.ObtenerProductos();
        Producto producto = productos.Find(p => p.codigoProducto == codigoProducto);

        if (producto == null)
        {
            Console.WriteLine("Producto no encontrado.");
            return null;
        }

        if (producto.cantidadStock < cantidad)
        {
            Console.WriteLine("No hay suficiente stock para realizar la compra.");
            return null;
        }

        producto.cantidadStock -= cantidad;
        registro.ActualizarProductos(productos);

        Console.WriteLine($"Has comprado {cantidad} unidades de {producto.nombreProducto}.");
        return producto;
    }
}

class Pago
{
    public void ProcesarPago(double monto)
    {
        Console.WriteLine($"El monto total a pagar es: {monto}");
        Console.WriteLine("Ingrese el monto con el que desea pagar:");
        double pago = double.Parse(Console.ReadLine());

        if (pago < monto)
        {
            Console.WriteLine("El monto ingresado es insuficiente. Intente de nuevo.");
        }
        else
        {
            double cambio = pago - monto;
            Console.WriteLine($"Pago realizado con éxito. Su cambio es: {cambio}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Registro miRegistro = new Registro("inventario.json");
        Compra miCompra = new Compra();
        Pago miPago = new Pago();

        while (true)
        {
            Console.WriteLine("\nCodigo del producto:");
            string entradaCodigo = Console.ReadLine();
            if (entradaCodigo.ToLower() == "salir") break;

            if (!int.TryParse(entradaCodigo, out int codigoProducto))
            {
                Console.WriteLine("Código de producto inválido. Intente de nuevo.");
                continue;
            }

            Console.WriteLine("Nombre del producto:");
            string nombreProducto = Console.ReadLine();

            Console.WriteLine("Cantidad en stock:");
            if (!int.TryParse(Console.ReadLine(), out int cantidadStock))
            {
                Console.WriteLine("Cantidad en stock inválida. Intente de nuevo.");
                continue;
            }

            Console.WriteLine("Precio del producto:");
            if (!double.TryParse(Console.ReadLine(), out double precioProducto))
            {
                Console.WriteLine("Precio del producto inválido. Intente de nuevo.");
                continue;
            }

            Producto producto = new Producto(codigoProducto, nombreProducto, cantidadStock, precioProducto);
            miRegistro.GuardarProducto(producto);
        }

        miRegistro.MostrarInfo();

        while (true)
        {
            Console.WriteLine("Desea comprar algun producto? (s/n)");
            string respuesta = Console.ReadLine().ToLower();
            if (respuesta == "n") break;

            Console.WriteLine("Ingrese el código del producto que desea comprar:");
            if (!int.TryParse(Console.ReadLine(), out int codigoProducto))
            {
                Console.WriteLine("Código inválido.");
                continue;
            }

            Console.WriteLine("Ingrese la cantidad que desea comprar:");
            if (!int.TryParse(Console.ReadLine(), out int cantidad))
            {
                Console.WriteLine("Cantidad inválida.");
                continue;
            }

            Producto productoComprado = miCompra.ComprarProducto(codigoProducto, cantidad, miRegistro);
            if (productoComprado != null)
            {
                double total = productoComprado.precioProducto * cantidad;
                miPago.ProcesarPago(total);
            }
        }
    }
}