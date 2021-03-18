#Best-Hack-API - бэкенд проекта по поиску лучших товаров

Для получения рекомендаций необходимо отправить Get-запрос на
   /api/store
указав параметр query - название товара.

В ответ сервер возвращает JSON в формате:

{
  "category": "Техника",
  "total": 1,
  "list": [
    {
      "name": "Смартфон Apple iPhone 12 128GB Blue (MGJE3RU/A)",
      "rating": 5,
      "popularity": 48,
      "averagePrice": 84990,
      "logo": "https://static.eldorado.ru/photos/71/715/665/47/new_71566547_l_1602620544.jpeg/resize/200x200/",
      "markets": [
        {
          "name": "Эьдорадо",
          "description": "Магазин электроники",
          "logo": "https://static.beeline.ru/upload/images/b2c/bee-logo/single.png",
          "price": 84990,
          "link": "https://www.eldorado.ru/cat/detail/smartfon-apple-iphone-12-128gb-blue-mgje3ru-a/"
        }
      ]
    }
  ]
}

где category - название категории товара, получаемое нашей обученной моделью ML.Net,
total - общее количество предлагаемых товаров (предлагаемые товары получаются, исходя из нашего алгоритма группировки выборок товаров с сайтов, т.е. мы объединяем товары в кластеры),
list - набор предлагаемых товаров,
name - название предлагаемого товара,
rating - 5-бальная оценка данного предлагаемого товара,
popularity - 100-бальная оценка популярности предлагаемого товара,
averagePrice - средняя цена за предлагаемый товар,
logo - ссылка на изображение предлагаемого товара,
markets - набор магазинов, содержащих предлагаемый товар,
name - название магазина, содержащего предлагаемый товар,
description - описание магазина, содержащего предлагаемый товар,
logo - ссылка на логотип магазина, содержащего предлагаемый товар,
price - стоимость предлагаемого товара из данного магазина,
link - ссылка на предлагаемый товар из данного магазина.

#ВНИМАНИЕ!!!
Для работы проекта необходимо дополнительно загрузить модель ML.Net, поскольку файл не проходит ограничение на размер файлов Github:
  Ссылка на Model.zip: https://bit.ly/3rVTuMu
