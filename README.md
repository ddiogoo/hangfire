# HangFire

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0) ![.Net](https://img.shields.io/badge/.NET-5C2D91?logo=.net&logoColor=white) ![Visual Studio](https://img.shields.io/badge/Visual%20Studio-5C2D91.svg?logo=visual-studio&logoColor=white)

Building Background Services in .NET with HangFire.

## Type of Background Services

### Background Tasks

- On demand:
    - Process that need to occur now;
    - Might be a workflow;
    - Examples:
        - Transactional Emails;
        - Payment Processing;
        - Webhooks;
        - Entity Processing.

- Recurring or Scheduled Task:
    - Tasks that need to happen on a schedule;
    - Hourly, Daily, Weekly, etc;
    - Examples:
        - Reports;
        - Resource Cleanup.