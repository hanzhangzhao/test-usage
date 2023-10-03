import {Deployment} from './main';
import {Testing} from 'cdk8s';

describe('Kubernetes Deployment', () => {
  test('Kubernetes manifest', () => {
    const app = Testing.app();
    const chart = new Deployment(app, 'test-chart');
    const results = Testing.synth(chart)
    expect(results).toMatchSnapshot();
  });
});
