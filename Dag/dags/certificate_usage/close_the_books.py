from airflow.decorators import dag, task
from airflow.providers.http.hooks.http import HttpHook
from datetime import datetime, timedelta

doc_md = """
## Close the Books (ERL)

Close the books on the current billing period, no new charges can be added to
the current billing period after this job is run. And additional charges,
reversals or corrections will appear on the next billing period's ERL.

Source: http://gitlab.com/sterlingcapitalbrokers/certificate_usage/Dag/dags/certificate_usage/close_the_books.py
"""

tags = ('finance', 'certificate_usage', 'close_the_books')

def ymd():
  tomorrow = datetime.today() + timedelta(days=1)
  return (tomorrow.year, tomorrow.month, 1)

@task
def close_the_books(carrier):
  year, month, day = ymd()
  endpoint = f"/v0/usage/close/carrier/{carrier}/year/{year}/month/{month}/day/{day}"

  print(f"certificate usage: closing the book by calling: {endpoint}")

  hook = HttpHook(http_conn_id='http_certificate_usage', method='POST')
  response = hook.run(endpoint=endpoint)
  return response.text

@dag(schedule='55 00 L * *', catchup=False, doc_md=doc_md, tags=tags + ('AIG',), start_date=datetime(2023, 7, 15))
def aig_close_the_books():
  close_the_books(carrier='AIG')

@dag(schedule='55 00 L * *', catchup=False, doc_md=doc_md, tags=tags + ('blue_cross',), start_date=datetime(2023, 7, 15))
def blue_cross_close_the_books():
  close_the_books(carrier='blue_cross')

@dag(schedule='55 00 L * *', catchup=False, doc_md=doc_md, tags=tags + ('canadalife',), start_date=datetime(2023, 7, 15))
def canadalife_close_the_books():
  close_the_books(carrier='canadalife')

@dag(schedule='55 00 L * *', catchup=False, doc_md=doc_md, tags=tags + ('equitable_life',), start_date=datetime(2023, 7, 15))
def equitable_life_close_the_books():
  close_the_books(carrier='equitable_life')

@dag(schedule='55 00 L * *', catchup=False, doc_md=doc_md, tags=tags + ('fenchurch',), start_date=datetime(2023, 7, 15))
def fenchurch_close_the_books():
  close_the_books(carrier='fenchurch')

@dag(schedule='55 00 L * *', catchup=False, doc_md=doc_md, tags=tags + ('pacific_blue_cross', 'pbc'), start_date=datetime(2023, 7, 15))
def pacific_blue_cross_close_the_books():
  close_the_books(carrier='pacific_blue_cross')

@dag(schedule='55 00 L * *', catchup=False, doc_md=doc_md, tags=tags + ('wawanesa',), start_date=datetime(2023, 7, 15))
def wawanesa_close_the_books():
  close_the_books(carrier='wawanesa')

aig_close_the_books()
blue_cross_close_the_books()
canadalife_close_the_books()
equitable_life_close_the_books()
fenchurch_close_the_books()
pacific_blue_cross_close_the_books()
wawanesa_close_the_books()
