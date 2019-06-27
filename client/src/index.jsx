import React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';

import {config, store} from './_helpers';
import { App } from './App';
import { sockets } from './Hubs/sockets';

render(
    <Provider store={store}>
        <App />
    </Provider>,
    document.getElementById('app')
);