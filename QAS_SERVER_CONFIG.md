# 🖥️ Configuración Servidor QAS - Registro de Mantenciones

## 📋 Información del Servidor

- **URL**: `http://portalqas:91`
- **Página Principal**: `http://portalqas:91/default.aspx`
- **Aplicación**: Sistema de Registro de Mantenciones Toyota
- **Entorno**: QAS (Quality Assurance Server)

## ⚙️ Configuración Actual

### Archivos Actualizados

#### `cypress.env.json`
```json
{
  "baseUrl": "http://portalqas:91",
  "testUser": "USERPRUEBA2",
  "testPassword": "1234",
  "validRut": "8249882-6",
  "validStock": "0000906403",
  "dbHost": "localhost",
  "enableSAPIntegration": false,
  "isCI": false
}
```

#### `cypress.config.js`
```javascript
baseUrl: process.env.CYPRESS_baseUrl || 'http://portalqas:91',
```

## 🧪 Pruebas de Conectividad

### Ejecutar Prueba de Conectividad
```bash
# Prueba específica de conectividad
npm run test:connectivity

# O ejecutar directamente con Cypress
npx cypress run --spec "cypress/e2e/connectivity/server-connectivity.cy.js"
```

### Pruebas Incluidas
1. **Acceso a página principal** - Verifica conectividad básica
2. **Acceso a página de login** - Verifica `/default.aspx`
3. **Resolución DNS** - Verifica que `portalqas` se resuelve correctamente
4. **Diagnóstico completo** - Información detallada para troubleshooting

## 🔧 Troubleshooting

### Problemas Comunes

#### 1. Error: "getaddrinfo ENOTFOUND portalqas"
**Causa**: El nombre `portalqas` no se puede resolver
**Soluciones**:
- Verificar que estás en la red corporativa
- Verificar configuración DNS
- Agregar entrada en `hosts` file si es necesario:
  ```
  # Windows: C:\Windows\System32\drivers\etc\hosts
  # Linux/Mac: /etc/hosts
  192.168.x.x portalqas
  ```

#### 2. Error: "connect ECONNREFUSED"
**Causa**: El servidor no está respondiendo en el puerto 91
**Soluciones**:
- Verificar que el servidor QAS está ejecutándose
- Verificar que el puerto 91 no está bloqueado por firewall
- Contactar al equipo de infraestructura

#### 3. Error: "timeout of 60000ms exceeded"
**Causa**: El servidor responde muy lento
**Soluciones**:
- Verificar conectividad de red
- Aumentar timeouts en `cypress.config.js`
- Verificar carga del servidor

#### 4. Error: "403 Forbidden" o "401 Unauthorized"
**Causa**: Problemas de autenticación/autorización
**Soluciones**:
- Verificar credenciales en `cypress.env.json`
- Verificar que el usuario tiene permisos
- Contactar al administrador del sistema

### Comandos de Diagnóstico

```bash
# Verificar conectividad básica (desde línea de comandos)
ping portalqas

# Verificar puerto específico (Windows)
telnet portalqas 91

# Verificar puerto específico (Linux/Mac)
nc -zv portalqas 91

# Verificar desde navegador
# Abrir: http://portalqas:91/default.aspx
```

## 🌐 Configuración de Red

### Requisitos de Red
- **Acceso a red corporativa**: Requerido
- **Puerto 91**: Debe estar abierto
- **DNS**: Debe resolver `portalqas`
- **Proxy**: Configurar si es necesario

### Variables de Entorno para Proxy (si aplica)
```bash
# Si la red corporativa requiere proxy
export HTTP_PROXY=http://proxy.empresa.com:8080
export HTTPS_PROXY=http://proxy.empresa.com:8080
export NO_PROXY=portalqas,localhost,127.0.0.1
```

## 📊 Monitoreo y Logs

### Logs de Cypress
Los logs detallados se pueden ver en:
- **Modo interactivo**: Consola del navegador en Cypress Test Runner
- **Modo headless**: Terminal donde se ejecuta el comando

### Información de Debug
```javascript
// En las pruebas, se incluye información como:
cy.log('🔗 Verificando conectividad con: ' + Cypress.config('baseUrl'));
cy.log('📊 Respuesta del servidor: ' + response.status);
cy.log('⏱️ Tiempo de carga: ' + loadTime + 'ms');
```

## 🔄 Actualización de Configuración

### Para cambiar servidor
1. Actualizar `cypress.env.json`:
   ```json
   {
     "baseUrl": "http://nuevo-servidor:puerto"
   }
   ```

2. Actualizar `cypress.config.js` si es necesario

3. Ejecutar prueba de conectividad:
   ```bash
   npm run test:connectivity
   ```

### Para cambiar credenciales
1. Actualizar en `cypress.env.json`:
   ```json
   {
     "testUser": "nuevo_usuario",
     "testPassword": "nueva_password"
   }
   ```

2. Ejecutar pruebas de autenticación:
   ```bash
   npm run test:auth
   ```

## 📞 Contactos

### Soporte Técnico
- **Infraestructura**: Para problemas de conectividad/servidor
- **Desarrollo**: Para problemas de aplicación
- **QA**: Para problemas de configuración de pruebas

### Escalación
1. **Nivel 1**: Verificar configuración local
2. **Nivel 2**: Verificar conectividad de red
3. **Nivel 3**: Contactar infraestructura/desarrollo

---

## ✅ Checklist de Verificación

Antes de ejecutar pruebas completas, verificar:

- [ ] Conectividad básica con `npm run test:connectivity`
- [ ] Credenciales correctas en `cypress.env.json`
- [ ] Acceso a red corporativa
- [ ] Puerto 91 accesible
- [ ] DNS resuelve `portalqas`
- [ ] Aplicación responde en `/default.aspx`

---

**Última actualización**: $(date)
**Configurado para**: Servidor QAS Toyota Chile
