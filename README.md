### Этап 3 — Валидация моделей, обработка ошибок, логирование

* Во все модели добавлены проверки на наличие некорректных данных
![Model Validation](img/ModelValidation.PNG)

* В файле Program.cs включена обработка ошибок HTTP:
![Http Error](img/HttpError.PNG)

и исключений:  
![Exception](img/Exception.png)

Ошибки пользователя отображаются отдельным представлением:  
![User Error](img/UserError.PNG)

* Настроено логирование через библиотеку NLog:
![Logger](img/Logger.PNG)

### Этап 2 — Разработка представлений

* Созданы представления для сущностей _Пользователь_, _Роль_, _Статья_, _Тег_, _Комментарий_.
* Для каждой сущности в проекте LvmBlog созданы контроллеры, реализующие CRUD операции.
* Тестировать представления удобнее под учетной записью admin/123

![Stage1](img/stage2.PNG)


### Этап 1 — Проектирование и разработка бэка

* Созданы сущности _Пользователь_, _Роль_, _Статья_, _Тег_, _Комментарий_ в базе данных.
* Для каждой сущности созданы контроллеры, реализующие CRUD операции.
* С целью тестирования БД контроллеры созданы пока только в проекте BlogApi. 
Бизнес-сущности находятся в каталоге SF.BlogApi/Contracts

![Stage1](img/stage1.PNG)