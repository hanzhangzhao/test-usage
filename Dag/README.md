# Monthly Billing Dag
> ETL to extract billing data, transform to netsuite contract & load into s3

Leverage airflow dags to implement a monthly billing dag.

## Installation

OS X & Linux:

```sh
pip install virtualenv
python -m venv venv
source venv/bin/activate
pip install -r requirements.txt
```

## Development setup

### Run tests
```sh
pytest tests
```

### Run locally

```sh
docker-compose up
```