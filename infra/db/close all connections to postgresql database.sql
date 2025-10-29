SELECT pg_terminate_backend(pid)
FROM pg_stat_activity
WHERE datname = 'Virtuelly' AND leader_pid IS NULL;