# StockMaster

Sistema de inventario desarrollado en ASP.NET Core para la gestión de productos, categorías y órdenes.

## Tecnologías usadas 

- ASP.NET Core 8.0 (MVC)
- Entity Framework Core
- SQL Server
- Razor Pages
- Rotativa (Generación de PDF)

## Funcionalidades 

- **Categorías**
  - Crear, editar y eliminar categorías.

- **Productos**
  - Gestión completa de productos.
  - Relación con categorías.
  - Control de stock.

- **Órdenes**
  - Creación y consulta de órdenes.
  - Validación de stock en tiempo real.
  - Eliminación de órdenes.

- **Generación de PDF**
  - Exportación de órdenes en formato PDF.
  - Incluye detalle de productos y totales.

- **Autenticación**
  - Login con correo y contraseña.
  - Protección de rutas (solo usuarios autenticados).

- **Registro de usuarios**
  - Alta de nuevos usuarios en el sistema.

## Requisitos 

- .NET 6/7/8
- SQL Server (LocalDB, Express o instancia local)

## Ejecución

Al ejecutar el proyecto:
- La base de datos se crea automáticamente
- Se aplican las migraciones de Entity Framework.

## Configuración

Opcionalmente, puedes modificar la cadena de conexión en:

'appsettings.json'

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=db_StockMaster;Trusted_Connection=True;"
  }
}
