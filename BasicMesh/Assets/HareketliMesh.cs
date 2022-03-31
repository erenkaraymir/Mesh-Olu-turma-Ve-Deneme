using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter),typeof(MeshCollider))]
public class HareketliMesh : MonoBehaviour
{
    Vector3[] vertices;
    int[] triangles;

    Mesh mesh;
    MeshCollider meshCollider;

    public float kareBoyutu = 3;
    public int kareSayisi = 15;

    /*
    Her bir vertex'in yükseklik yani Y eksenindeki deðerlerini
    tutacaðýmýz ve hesaplayýp güncelleyeceðimiz dizi.
    */
    float[,] yukseklik;

    //Dersten biraz farklý olarak böyle bir þey deðiþken de ekledim.
    //Bu deðiþken sayesinde PerlinNoise derecesini ayarlayabiliyoruz.
    [Range(0, 1)]
    [SerializeField]
    float perlinScale = .75f;

    [Range(1, 15)]
    [SerializeField]
    float maksYukseklik = 7f;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();
        YukseklikVerisi();
        PlaneVerisiOlustur();
        MeshOlustur();
    }

    void Update()
    {
        YukseklikVerisi();
        PlaneVerisiOlustur();
        MeshOlustur();
    }

    //Plane(Düz Zemin) Mesh Objemizi Oluþturduðumuz Fonksiyon
    public void PlaneVerisiOlustur()
    {

        /*
        Burada kare sayýsýný bir fazla kullanma sebebimizi
        derslerde daha ayrýntýlý anlatmýþtým.
        Ancak basitçe bahsedersek,
        5x5 þeklinde bir grid oluþturursak,
        her bir satýr ve sütunda 6 tane noktadan(çizgilerin kesiþimi)
        olduðunu görürüz. 
        Yani kare sayýmýzdan 1 fazla
        */
        vertices = new Vector3[(kareSayisi + 1) * (kareSayisi + 1)];
        /*
        Her bir karemizin 6 tane vertexten oluþmasýný istiyoruz.
        Bu sayede ayrýk üçgenler dersimizdeki yapýyý kullanacaðýz.
        Triangle içerisinde de oluþturacaðýmýz mesh yaratýrken 
        oluþturduðumuz üçgenlerin noktalarý veriyoruz.
        Her bir üçgen 3 noktadan oluþuyor, 
        ve biz 2 üçgeni 1 kare olarak görüyoruz.
        Yani kaç kare kullanacaksak bunun için 6 adet de
        vertex deðerini triangles dizisinde tutacaðýz.
        Bu yüzden toplam kare sayýmýzý 6 ile çarpýyoruz.
        */
        triangles = new int[kareSayisi * kareSayisi * 6];

        /*
        Bir kareyi oluþtururken vertexlerin konumunu
        kare merkezinden yatay ve dikeyde istediðimiz boyutun yarýsý
        kadar olacaktýr. Bu yüzden .5f ile 
        yarýsýný bir deðiþkende tutuyoruz.
        */
        float vertexAralik = kareBoyutu * .5f;

        //Vertexleri oluþturmak ve dolaþmak için kullanacaðýmýz index deðiþkeni
        int v = 0;
        //Triangles dizisini oluþtururken kullanacaðýmýz index deðiþkeni
        int t = 0;

        //Ýçiçe for döngüsü sayesinde yatay ve dikeyde
        //vertexlerimizi oluþturuyoruz.
        for (int x = 0; x <= kareSayisi; x++)
        {
            for (int z = 0; z <= kareSayisi; z++)
            {
                /*
                X ve Z deðerleri ile kare boyutunu çarparak
                oluþturacaðýmýz karenin merkezini buluyoruz.
                Ardýndan da daha önce belirlediðimiz aralýðý çýkararak
                yaratacaðýmýz vertex pozisyonunu tam olarak belirlemiþ oluyoruz.
                Ayrýca yukseklik dizisine önceden atadýðýmýz deðerleri de Y eksenine veriyoruz.
                Ardýndan vertices dizimize bu pozisyon bilgisini kaydediyoruz.
                */
                vertices[v] = new Vector3((x * kareBoyutu) - vertexAralik, yukseklik[x, z], (z * kareBoyutu) - vertexAralik);
                /*
                Her döngü sonrasýnda v index deðiþkenimizi arttýrarak
                bir sonraki vertex deðerine geçip bunu kaydediyoruz.
                Ýçiçe döngü kullanýp tek boyutlu diziye veri kaydederken
                bu yöntem basit ve kullanýþlý. Farklý yöntemler de deneyebilirsiniz.
                */
                v++;
            }
        }

        v = 0;
        //Karelerin oluþturulma sýrasý - Mesh Görünümü
        /* 
        3 7 11 15
        2 6 10 14
        1 5 9  13
        0 4 8  12
        */
        //Karelerin dizi içerisinde görünümü
        //0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15

        //Bu kýsýmda kare sayýsýný içiçe for döngüsü ile
        //iki defa dönüyoruz.
        //Bu sayede kare sayýsý 5 girildiyse 
        //5x5 þeklinde kare bir mesh oluþturabiliyoruz.
        for (int x = 0; x < kareSayisi; x++)
        {
            for (int z = 0; z < kareSayisi; z++)
            {
                /*
                Burada yaptýðýmýz þey basitçe her bir kareyi dolaþmak
                ve karenin sahip olduðu 6 vertex deðerini kullanarak
                2 adet üçgen oluþturmak.
                Triangles dizisindeki deðerleri üçer üçer alarak
                üçgenleri oluþturduðumuzu unutmayýn.
                */
                triangles[t + 0] = v;
                triangles[t + 1] = v + 1;
                triangles[t + 2] = v + (kareSayisi + 1);
                triangles[t + 3] = v + (kareSayisi + 1);
                triangles[t + 4] = v + 1;
                triangles[t + 5] = v + (kareSayisi + 1) + 1;
                //Vertex deðerimizi arttýrarak bir sonraki vertex'e geçiyoruz.
                v++;
                /*
                Hali hazýrda 6 tane vertex verisini vererek
                2 adet üçgeni oluþturduðumuz için t index deðerimizi
                6 arttýrýp, diðer kareye geçiyoruz.
                */
                t += 6;
            }
            /*
            Daha önceki döngüden farklý olarak burada da vertex index
            deðerini arttýrmamýzýn sebebi sýnýra geliyor olmamýz.
            Bahsettiðim gibi, 5 kare için 6 tane nokta oluyor yatay ve dikeyde.
            Ancak en üstteki ve saðdaki vertexlerin devamýnda kareler oluþturmuyoruz.
            Bu yüzden bu deðeri arttýrarak o vertexleri atlayýp iþleme devam ediyoruz.
            */
            v++;
        }
    }

    public void YukseklikVerisi()
    {
        //Vertex sayýsýnýn neden karesayisindan 1 fazla olduðunu PlaneVerisiOlustur
        //fonksiyonu içerisinde bahsetmiþtim.
        yukseklik = new float[kareSayisi + 1, kareSayisi + 1];
        //Vertex aralik konusunda da ayný þekilde PlaneVerisiOlustur içerisinde bulabilirsiniz.
        float vertexAralik = kareBoyutu * .5f;

        //X ve Z eksenleri üzerinde ilerliyoruz.
        //2 boyutlu bir grid oluþturmuþtuk sonuçta aslýnda.
        for (int x = 0; x <= kareSayisi; x++)
        {
            for (int z = 0; z <= kareSayisi; z++)
            {
                /*
                x ve z deðerlerini kare boyutu ile çarpýp aralýk deðerini çýkararak
                vertex'in olacaðý konumu belirliyoruz.
                Bu deðerleri aslýnda PlaneVerisiOlustur içerisinde de tekrar hesaplýyoruz.
                Önce onu çaðýrýp, konumlarý hesaplatýp sonra yükseklik verisi hesapladýðýmýz 
                bir yapýyý kurmayý size ödev olarak býrakýyorum. 
                Bu haliyle ayný iþi 2 kere yapýyoruz sonuçta, daha optimize olmalý :)
                Ayrýca yalnýzca X ekseninde hareket etmesini istediðim için, geçen zamaný da ilk deðiþkene ekliyorum.
                */
                float xVerisi = (x * kareBoyutu) - vertexAralik + Time.time;
                float zVerisi = (z * kareBoyutu) - vertexAralik;
                /*Yüksekliðini belirleyeceðimiz vertex'in gerçek konum deðerlerini 
                PerlinNoise için deðiþken olarak veriyoruz.
                Bu fonksiyon bize 0 ve 1 arasý bir deðer verecek. maksYukseklik deðeriyle de çarparak bunu
                yukseklik adlý dizimize kaydediyoruz.
                */
                float vertexYuksekligi = Mathf.PerlinNoise(xVerisi * perlinScale, zVerisi * perlinScale);
                yukseklik[x, z] = vertexYuksekligi * maksYukseklik;
            }
        }
    }

    /*
    Burada yaptýðýmýz þey sýrasýyla þöyle
    Mesh verisini temizliyoruz.
    Mesh'i oluþturacak vertexlerin atamasýný yapýyoruz.
    Mesh'i oluþturacak üçgenlerin atamasýný yapýyoruz.
    Üçgenlerin Normal deðerlerini hesaplatýyoruz.
    Ve son olarak Collider bileþenimize bu oluþturduðumuz 
    mesh verisini ekliyoruz.
    */
    public void MeshOlustur()
    {
        if (mesh != null)
            mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
    }

}