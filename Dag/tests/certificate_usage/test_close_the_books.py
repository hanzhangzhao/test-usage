import pytest
from datetime import datetime, timedelta
from airflow.providers.http.hooks.http import HttpHook
from Dag.dags.certificate_usage.close_the_books import close_the_books

class MockConnection:
  def __init__(self) -> None:
    self.host = "127.0.0.1"
    self.port = 6379

  def sadd(*arg, **kwargs):
    pass

class MockResponse:
  def __init__(self, text):
    self.text = text

@pytest.fixture(autouse=True)
def mock(monkeypatch):
  monkeypatch.setattr(HttpHook, "get_conn", lambda: MockConnection())
  monkeypatch.setattr(HttpHook, "run", lambda *a, **k : MockResponse(k['endpoint']))

def test_fetch_erl():
  today = datetime.today() + timedelta(days=1)
  assert close_the_books.function("equitable_life") == f"/v0/usage/close/carrier/equitable_life/year/{today.year}/month/{today.month}/day/1"
