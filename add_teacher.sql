-- ============================================================
-- Запустить в PostgreSQL (схема "Diplom")
-- После выполнения: логин teacher1 / пароль 1234
-- ============================================================

-- 1. Добавить роль Teacher (если ещё нет)
INSERT INTO "Diplom".roles (name)
VALUES ('Teacher')
ON CONFLICT (name) DO NOTHING;

-- 2. Создать пользователя-учителя
INSERT INTO "Diplom".users (login, password, full_name, role_id)
SELECT 'teacher1', '1234', 'Teacher User', r.id
FROM "Diplom".roles r
WHERE r.name = 'Teacher'
ON CONFLICT (login) DO NOTHING;

-- 3. Проверка
SELECT u.id, u.login, u.full_name, r.name AS role
FROM "Diplom".users u
JOIN "Diplom".roles r ON r.id = u.role_id
WHERE r.name = 'Teacher';
