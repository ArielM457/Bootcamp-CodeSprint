# **Sesión 00: instalación del entorno**

Esta guía prepara Windows para ejecutar el frontend con Node.js y crear el backend con C#, .NET 10, ASP.NET Core y SignalR.

## **1. Instalar Node.js, JavaScript y npm**

JavaScript no se instala por separado. Node.js permite ejecutar JavaScript fuera del navegador y su instalador incluye npm, que sirve para descargar las dependencias del frontend.

Puedes descargar la versión LTS desde la página oficial:

- https://nodejs.org/en/download

De igual forma, puedes instalarla desde PowerShell con WinGet:

```powershell
winget install --id OpenJS.NodeJS.LTS --exact
```

Cierra y abre la terminal. Después comprueba la instalación:

```powershell
node --version
npm --version
```

## **2. Instalar .NET 10 y C#**

El SDK de .NET 10 incluye el compilador de C#, ASP.NET Core y la herramienta `dotnet`. No necesitas instalar C# por separado.

En PowerShell con permisos de administrador, ejecuta:

```powershell
winget install --id Microsoft.DotNet.SDK.10 --exact --source winget
```

Después abre una terminal nueva y comprueba la instalación:

```powershell
dotnet --version
dotnet --list-sdks
```

La lista debe mostrar una versión `10.0.x`.

Documentación oficial:

- https://learn.microsoft.com/dotnet/core/install/windows

## **3. Preparar SignalR**

SignalR permite comunicación en tiempo real entre el backend y el frontend.

La parte servidor ya está incluida en ASP.NET Core. Cuando creemos un Hub en una sesión posterior podremos usar `Microsoft.AspNetCore.SignalR` sin instalar otro paquete en el backend.

El cliente para JavaScript y TypeScript se instala con npm:

```powershell
cd "F:\Proyectos\Bootcamp - Bomberman\frontend"
npm install @microsoft/signalr
```

En este frontend la dependencia ya está declarada en `package.json`, por lo que `npm ci` también la instala.

Documentación oficial:

- https://learn.microsoft.com/aspnet/core/signalr/javascript-client

Esta sesión solo prepara la herramienta. 

## **4. Instalar y ejecutar el frontend**

Abre PowerShell y ejecuta:

```powershell
cd "F:\Proyectos\Bootcamp - Bomberman\frontend"
npm ci
npm start
```

Después abre esta dirección:

```text
http://localhost:4200
```

Usa `Ctrl+C` en la terminal para detener el servidor.

## **5. Ejecutar el backend de la sesión 1**

En otra terminal de PowerShell, ejecuta:

```powershell
cd "F:\Proyectos\Bootcamp - Bomberman\sesiones - backend\sesion 1"
dotnet restore
dotnet run --launch-profile http
```

Después abre esta dirección:

```text
http://localhost:5268/api/hello
```

Usa `Ctrl+C` en la terminal para detener la API.

## **6. Verificación rápida**

Ejecuta estos comandos para confirmar que las herramientas principales están disponibles:

```powershell
node --version
npm --version
dotnet --version
```

En el equipo donde preparamos estas sesiones verificamos Node.js `24.12.0`, npm `11.6.2` y .NET SDK `10.0.101`.
