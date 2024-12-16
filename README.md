# IsTakipSistemi
IsTakipSistemi asp.net project 

# IsTakip Sistemi MVC

Bu proje, **ASP.NET 4.7 ve üzeri** sürümünde geliştirilmiş bir MVC uygulamasıdır. Bu projede SQL Server kullanılmıştır ve veritabanı bağlantısı **ADO.NET** ile yapılmıştır.

## Veritabanı Yükleme ve Bağlantı Ayarları

Proje için veritabanı yedeği (backup) dosyasını **`backup.bak`** olarak bu repo içerisinde bulabilirsiniz. Bu adımları izleyerek veritabanını kurabilir ve projeyi çalıştırabilirsiniz.

### 1. **Veritabanı Yedeği (backup.bak) Dosyasını Yükleme**

1. **SQL Server Management Studio (SSMS)** uygulamasını açın.
2. **Object Explorer** kısmında **Databases** üzerine sağ tıklayın ve **Restore Database...** seçeneğini tıklayın.
3. Restore Database penceresinde:
   - **Source** kısmında **Device**'i seçin.
   - **Add** butonuna tıklayarak `backup.bak` dosyasını ekleyin.
   - **Destination** kısmında, yüklemek istediğiniz veritabanının adını belirtin. Örneğin, **IsTakipDB**.
   - Restore işlemini başlatın ve tamamlayın.

Veritabanı yedeği başarıyla yüklendiğinde, bu veritabanını projede kullanabilirsiniz.

### 2. **Connection String'i Düzenleme**

Projeyi çalıştırabilmek için **web.config** dosyasındaki **Connection String**'i doğru şekilde ayarlamanız gerekmektedir.

- `web.config` dosyasındaki **connectionStrings** bölümünü bulun ve aşağıdaki gibi düzenleyin:

```xml
<connectionStrings>
  <add name="IsTakipDBEntities" 
       connectionString="metadata=res://*/Models.IsTakipModel.csdl|res://*/Models.IsTakipModel.ssdl|res://*/Models.IsTakipModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=YOUR_SERVER_NAME\SQLEXPRESS;initial catalog=IsTakipDB;integrated security=True;trustservercertificate=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" 
       providerName="System.Data.EntityClient" />
</connectionStrings>

