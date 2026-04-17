-- seed_data.sql
-- Тестовые данные для базы данных matyedb
-- Выполнить: psql -U postgres -d matyedb -f seed_data.sql

-- Роли
INSERT INTO roles ("roleName") VALUES
('Пользователь'),
('Мастер'),
('Модератор'),
('Администратор');

-- Пользователи (пароли plain text)
INSERT INTO users ("login", "password", "email", "phone", "balance", "roleId", "createdAt") VALUES
('admin',     'admin123',  'admin@matye.ru',    '+79001110001', 0,      4, NOW()),
('moderator', 'moder123',  'moder@matye.ru',    '+79001110002', 0,      3, NOW()),
('master1',   'master123', 'ivan@matye.ru',     '+79001110003', 0,      2, NOW()),
('master2',   'master123', 'anna@matye.ru',     '+79001110004', 0,      2, NOW()),
('master3',   'master123', 'oleg@matye.ru',     '+79001110005', 0,      2, NOW()),
('user1',     'user123',   'user1@mail.ru',     '+79001110006', 1500,   1, NOW()),
('user2',     'user123',   'user2@mail.ru',     '+79001110007', 500,    1, NOW()),
('user3',     'user123',   'user3@mail.ru',     '+79001110008', 2000,   1, NOW()),
('user4',     'user123',   'user4@mail.ru',     '+79001110009', 0,      1, NOW()),
('user5',     'user123',   'user5@mail.ru',     '+79001110010', 750,    1, NOW());

-- Мастера
INSERT INTO masters ("userId", "qualificationLevel", "bio") VALUES
(3, 3, 'Специалист по кастомизации одежды и аниме-косплею. Опыт 5 лет.'),
(4, 2, 'Мастер по созданию костюмов и аксессуаров. Специализация — Хэллоуин и Нуар.'),
(5, 1, 'Начинающий мастер. Специализация — новогодние украшения и киберпанк.');

-- Категории
INSERT INTO categories ("categoryName") VALUES
('Кастомизация'),
('Косплей'),
('Праздники');

-- Коллекции
INSERT INTO collections ("collectionName") VALUES
('Аниме'),
('Новый год'),
('Хэллоуин'),
('Киберпанк'),
('Нуар');

-- Услуги
INSERT INTO services ("serviceName", "description", "price", "categoryId", "collectionId", "imageUrl", "createdAt", "updatedAt") VALUES
('Кастом куртка Аниме',        'Роспись куртки по мотивам аниме на выбор клиента',           3500, 1, 1, 'Assets/pr/Кастом/Pr1.jpg',   NOW(), NOW()),
('Кастом кроссовки Аниме',     'Ручная роспись кроссовок в аниме-стиле',                     2800, 1, 1, 'Assets/pr/Кастом/Pr2.jpg',   NOW(), NOW()),
('Кастом сумка Аниме',         'Декорирование сумки персонажами аниме',                      1800, 1, 1, 'Assets/pr/Кастом/Pr3.jpg',   NOW(), NOW()),
('Кастом футболка Киберпанк',  'Роспись футболки в стиле киберпанк',                         1500, 1, 4, 'Assets/pr/Кастом/Pr4.jpg',   NOW(), NOW()),
('Кастом куртка Нуар',         'Кастомизация куртки в стиле нуар с монохромным рисунком',    3200, 1, 5, 'Assets/pr/Кастом/Pr5.jpg',   NOW(), NOW()),
('Кастом обувь Хэллоуин',      'Роспись обуви в хэллоуинской тематике',                      2200, 1, 3, 'Assets/pr/Кастом/Pr6.jpg',   NOW(), NOW()),
('Кастом рюкзак Новый год',    'Декорирование рюкзака новогодними мотивами',                 1600, 1, 2, 'Assets/pr/Кастом/Pr7.jpg',   NOW(), NOW()),
('Кастом джинсы Аниме',        'Роспись джинсов по мотивам любимого аниме',                  2500, 1, 1, 'Assets/pr/Кастом/Pr8.jpg',   NOW(), NOW()),
('Кастом кепка Киберпанк',     'Кастомизация кепки в стиле киберпанк',                       900,  1, 4, 'Assets/pr/Кастом/Pr9.jpg',   NOW(), NOW()),
('Кастом пальто Нуар',         'Роспись пальто в нуар-стиле',                                4500, 1, 5, 'Assets/pr/Кастом/Pr10.jpg',  NOW(), NOW()),
('Кастом маска Хэллоуин',      'Создание и роспись маски для Хэллоуина',                     1200, 1, 3, 'Assets/pr/Кастом/Pr11.jpg',  NOW(), NOW()),
('Кастом шарф Новый год',      'Вязаный шарф с новогодним орнаментом на заказ',              800,  1, 2, 'Assets/pr/Кастом/Pr12.jpg',  NOW(), NOW()),
('Косплей Наруто',             'Полный костюм Наруто: кимоно, протектор, повязка',           5500, 2, 1, 'Assets/pr/Косплей/KL1.jpg',  NOW(), NOW()),
('Косплей Сейлор Мун',         'Костюм Сейлор Мун с аксессуарами',                          6200, 2, 1, 'Assets/pr/Косплей/KL2.jpg',  NOW(), NOW()),
('Косплей Ведьма Хэллоуин',    'Костюм ведьмы с плащом, шляпой и метлой',                   4800, 2, 3, 'Assets/pr/Косплей/KL3.jpg',  NOW(), NOW()),
('Косплей Киберпанк самурай',  'Костюм самурая в стиле киберпанк',                          7000, 2, 4, 'Assets/pr/Косплей/KL4.jpg',  NOW(), NOW()),
('Косплей Детектив Нуар',      'Костюм детектива в стиле нуар: плащ, шляпа, галстук',       5800, 2, 5, 'Assets/pr/Косплей/KL5.jpg',  NOW(), NOW()),
('Косплей Снегурочка',         'Костюм Снегурочки с кокошником и шубкой',                   5200, 2, 2, 'Assets/pr/Косплей/KL6.jpg',  NOW(), NOW()),
('Косплей Дракула',            'Костюм Дракулы с плащом, жилетом и клыками',                4500, 2, 3, 'Assets/pr/Косплей/KL7.jpg',  NOW(), NOW()),
('Новогодний декор интерьера', 'Создание новогодних украшений для дома на заказ',            3000, 3, 2, '',                           NOW(), NOW());

-- Привязка услуг к мастерам
INSERT INTO "masterServices" ("masterId", "serviceId") VALUES
(1, 1), (1, 2), (1, 3), (1, 8), (1, 13), (1, 14),
(2, 6), (2, 11), (2, 15), (2, 17), (2, 19),
(3, 4), (3, 7), (3, 9), (3, 12), (3, 16), (3, 18), (3, 20);

-- Записи на услуги
INSERT INTO appointments ("userId", "masterServiceId", "appointmentDate", "queueNumber", "status", "createdAt") VALUES
(6,  1,  NOW() + INTERVAL '1 day',  1, 'Ожидание',   NOW()),
(7,  2,  NOW() + INTERVAL '2 days', 1, 'Ожидание',   NOW()),
(8,  6,  NOW() + INTERVAL '1 day',  1, 'Ожидание',   NOW()),
(9,  13, NOW() - INTERVAL '5 days', 1, 'Завершено',  NOW() - INTERVAL '5 days'),
(10, 15, NOW() - INTERVAL '3 days', 1, 'Завершено',  NOW() - INTERVAL '3 days'),
(6,  7,  NOW() + INTERVAL '3 days', 2, 'Ожидание',   NOW()),
(7,  11, NOW() - INTERVAL '1 day',  2, 'В процессе', NOW() - INTERVAL '1 day'),
(8,  16, NOW() + INTERVAL '4 days', 1, 'Ожидание',   NOW()),
(9,  3,  NOW() - INTERVAL '7 days', 2, 'Завершено',  NOW() - INTERVAL '7 days'),
(10, 18, NOW() + INTERVAL '5 days', 1, 'Ожидание',   NOW());

-- Отзывы
INSERT INTO reviews ("userId", "serviceId", "masterId", "rating", "comment", "createdAt") VALUES
(6,  1,    NULL, 5, 'Отличная работа! Куртка получилась именно такой, как я хотел.',         NOW() - INTERVAL '4 days'),
(7,  NULL, 1,    4, 'Мастер очень внимательный, всё объяснил. Результат понравился.',        NOW() - INTERVAL '3 days'),
(8,  6,    NULL, 5, 'Обувь выглядит потрясающе, все спрашивают где делала!',                 NOW() - INTERVAL '2 days'),
(9,  13,   NULL, 5, 'Костюм Наруто сшит идеально, очень доволен качеством.',                NOW() - INTERVAL '4 days'),
(10, 15,   NULL, 4, 'Хороший костюм, но немного задержали срок.',                           NOW() - INTERVAL '2 days'),
(6,  NULL, 2,    5, 'Анна — профессионал своего дела. Рекомендую!',                         NOW() - INTERVAL '1 day'),
(7,  3,    NULL, 3, 'Сумка красивая, но краска немного потёрлась через неделю.',             NOW() - INTERVAL '6 days'),
(8,  NULL, 3,    4, 'Олег старается, хорошая работа для начинающего мастера.',              NOW() - INTERVAL '5 days'),
(9,  NULL, 1,    5, 'Иван — лучший мастер! Уже третий раз заказываю.',                      NOW() - INTERVAL '3 days'),
(10, 7,    NULL, 4, 'Рюкзак получился очень праздничным, всем советую.',                    NOW() - INTERVAL '1 day');

-- Заявки на повышение квалификации
INSERT INTO "qualificationRequests" ("masterId", "requestDate", "status") VALUES
(3, NOW() - INTERVAL '10 days', 'Одобрено'),
(2, NOW() - INTERVAL '5 days',  'На рассмотрении'),
(3, NOW() - INTERVAL '1 day',   'На рассмотрении');

-- Пополнения баланса
INSERT INTO payments ("userId", "amount", "cardNumber", "paymentDate") VALUES
(6,  1500, '4321', NOW() - INTERVAL '10 days'),
(7,  500,  '8765', NOW() - INTERVAL '8 days'),
(8,  2000, '1234', NOW() - INTERVAL '6 days'),
(10, 750,  '9999', NOW() - INTERVAL '3 days'),
(6,  500,  '4321', NOW() - INTERVAL '2 days'),
(9,  1000, '5678', NOW() - INTERVAL '1 day'),
(7,  300,  '8765', NOW() - INTERVAL '4 days'),
(8,  500,  '1234', NOW() - INTERVAL '1 day'),
(10, 250,  '9999', NOW() - INTERVAL '5 days'),
(6,  200,  '4321', NOW());
