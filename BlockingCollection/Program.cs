using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace BlockingCollection
{//ConcurrentCollections2
 //class Program
 //{
 //    static void Main(string[] args)
 //    {
 //        try
 //        {
 //            PersonManager manager = new PersonManager();
 //            manager.StartTest();
 //        }
 //        catch (Exception excp)
 //        {
 //            Console.WriteLine(excp.Message);
 //        }

    //        Console.ReadLine();
    //    }
    //}

    //// Metin dosyasındaki bilgilerin nesne karşılıkları için tasarlanmış Person sınıfı
    //class Person
    //{
    //    public int PersonId { get; set; }
    //    public string Name { get; set; }
    //    public string Title { get; set; }
    //    public decimal Salary { get; set; }
    //}

    // Test metodunu içeren Test sınıfımız
    //class PersonManager
    //{
    //    // Person bilgilerinin tutulacağı generic List koleksiyonu
    //    List<Person> personList = new List<Person>();

    //    public void StartTest()
    //    {
    //        // GetPersonList metodu için bir Thread tanımlanır
    //        Thread trd1 = new Thread(new ThreadStart(GetPersonList));
    //        // ProcessPersonList metodu için bir Thread tanımlanır
    //        Thread trd2 = new Thread(new ThreadStart(ProcessPersonList));

    //        // Thread' ler başlatılır
    //        trd1.Start();
    //        trd2.Start();
    //    }

    //    // Metin dosyasından okuma işlemini yaparak personList isimli generic List koleksiyonuna Person nesne örneklerinin eklenmesi işlemini üstlenir
    //    private void GetPersonList()
    //    {
    //        // Personel.txt dosyasındaki tüm satırlar string[] dizisine alınır
    //        string[] persons = File.ReadAllLines(System.Environment.CurrentDirectory + "\\Personel.txt");

    //        // Her bir satır ele alınır
    //        foreach (string person in persons)
    //        {
    //            // Satır | işaretine göre ayrıştırılır
    //            string[] values = person.Split('|');

    //            // Ayrıştırma sonucu elde edilen değerlere göre Person nesne örneği oluşturulur
    //            Person prs = new Person
    //            {
    //                PersonId = Convert.ToInt32(values[0]),
    //                Name = values[1],
    //                Title = values[2],
    //                Salary = Convert.ToDecimal(values[3])
    //            };
    //            // Persone nesne örneği koleksiyona eklenir
    //            personList.Add(prs);
    //            // Console penceresinden bilgilendirme yapılır
    //            Console.WriteLine("{0} listeye eklendi", prs.Name);

    //            Thread.Sleep(250); // işleyişi kolay takip edebilmek için küçük bir zaman aldatmacası
    //        }
    //    }

    //    // personList isimli generic List koleksiyonundaki her bir Person nesne örneğinin Salary bilgisini değiştirir
    //    private void ProcessPersonList()
    //    {
    //        // Koleksiyondaki her bir Persone nesne örneği ele alınır
    //        foreach (Person person in personList)
    //        {
    //            // O anki Person nesne örneğinin Salary özelliğinin değeri değiştirilir
    //            person.Salary += 1.18M;

    //            // Console ekranında bilgilendirme yapılır
    //            Console.WriteLine("\t {0} için maaş {1} olarak değiştirildi", person.Name, person.Salary);
    //            Thread.Sleep(250); // işleyişi kolay takip edebilmek için küçük bir zaman aldatmacası
    //        }
    //    }
    //}


    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                PersonManager manager = new PersonManager();
                manager.StartTestConcurrent();
            }
            catch (Exception excp)
            {
                Console.WriteLine(excp.Message);
            }
        }
    }

    // Metin dosyasındaki bilgilerin nesne karşılıkları için tasarlanmış Person sınıfı
    class Person
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public decimal Salary { get; set; }
    }

    // Test metodunu içeren Test sınıfımız
    class PersonManager
    {
        // Person bilgilerinin tutulacağı generic List koleksiyonu
        // List<Person> personList = new List<Person>();
        BlockingCollection<Person> personList = new BlockingCollection<Person>();

        public void StartTestConcurrent()
        {
            // Task' leri başlatalım
            Task[] tasks ={ Task.Factory.StartNew(() => { GetPersonList(); }),
                              Task.Factory.StartNew(() => { ProcessPersonList(); })
                          };

            // Tüm Task' ler tamamlanıncaya kadar bekle
            Task.WaitAll(tasks);

            Console.WriteLine("İşlemler sona erdi. Programdan çıkmak için bir tuşa basın");
            Console.ReadLine();
        }

        // Metin dosyasından okuma işlemini yaparak personList isimli generic List koleksiyonuna Person nesne örneklerinin eklenmesi işlemini üstlenir
        private void GetPersonList()
        {
            // Personel.txt dosyasındaki tüm satırlar string[] dizisine alınır
            string[] persons = File.ReadAllLines(System.Environment.CurrentDirectory + "\\Personel.txt");

            // Her bir satır ele alınır
            foreach (string person in persons)
            {
                // Satır | işaretine göre ayrıştırılır
                string[] values = person.Split('|');

                // Ayrıştırma sonucu elde edilen değerlere göre Person nesne örneği oluşturulur
                Person prs = new Person
                {
                    PersonId = Convert.ToInt32(values[0]),
                    Name = values[1],
                    Title = values[2],
                    Salary = Convert.ToDecimal(values[3])
                };
                // Persone nesne örneği koleksiyona eklenir
                personList.Add(prs);
                // Console penceresinden bilgilendirme yapılır
                Console.WriteLine("{0} listeye eklendi", prs.Name);

                Thread.Sleep(250); // işleyişi kolay takip edebilmek için küçük bir zaman aldatmacası
            }
            // koleksiyona daha fazla eleman eklenmeyeceğini belirt.
            // Bu metodu kullanmadan denediğinizde programın asılı kaldığını ve kapanmadığını göreceksiniz.
            personList.CompleteAdding();
        }

        // personList isimli generic List koleksiyonundaki her bir Person nesne örneğinin Salary bilgisini değiştirir
        private void ProcessPersonList()
        {
            // Koleksiyondaki her bir Persone nesne örneği ele alınır
            foreach (Person person in personList.GetConsumingEnumerable())
            {
                // O anki Person nesne örneğinin Salary özelliğinin değeri değiştirilir
                person.Salary += 1.18M;

                // Console ekranında bilgilendirme yapılır
                Console.WriteLine("\t {0} için maaş {1} olarak değiştirildi", person.Name, person.Salary);
                Thread.Sleep(250); // işleyişi kolay takip edebilmek için küçük bir zaman aldatmacası
            }
        }
    }
}


