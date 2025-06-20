<template>
  <div class="responses-page">
    <h1>Responses to My Tickets</h1>

    <div class="filter-bar">
      <input v-model="filters.search" type="text" placeholder="Search message..." />
      <select v-model="filters.status">
        <option value="">All Statuses</option>
        <option value="Open">Open</option>
        <option value="InProgress">In Progress</option>
        <option value="Resolved">Resolved</option>
        <option value="Closed">Closed</option>
        <option value="Reopened">Reopened</option>
        <option value="Deleted">Deleted</option>
      </select>
      <input v-model="filters.fromDate" type="date" />
      <input v-model="filters.toDate" type="date" />
    </div>

    <div v-if="filteredTickets.length === 0" class="no-tickets">No responses found.</div>

    <div class="responses-grid">
      <div v-for="ticket in filteredTickets" :key="ticket.id" class="ticket-card">
        <div class="ticket-card-header">
          <strong class="ticket-title">Ticket #{{ ticket.id }}</strong>
          <span class="ticket-date">{{ formatDateOnly(ticket.date) }}</span>
        </div>
        <p class="ticket-message">{{ ticket.message }}</p>
        <p class="ticket-status">Status: {{ ticket.status }}</p>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      filters: {
        search: '',
        status: '',
        fromDate: '',
        toDate: '',
      },
      tickets: [
        {
          id: 6,
          date: '2025-06-12T13:24:23',
          message: 'This will be done in short period of time',
          status: 'Resolved',
        },
      ],
    };
  },
  computed: {
    filteredTickets() {
      return this.tickets.filter((ticket) => {
        const searchMatch = ticket.message
          .toLowerCase()
          .includes(this.filters.search.toLowerCase());
        const statusMatch =
          !this.filters.status || ticket.status === this.filters.status;
        const fromDateMatch =
          !this.filters.fromDate || new Date(ticket.date) >= new Date(this.filters.fromDate);
        const toDateMatch =
          !this.filters.toDate || new Date(ticket.date) <= new Date(this.filters.toDate);

        return searchMatch && statusMatch && fromDateMatch && toDateMatch;
      });
    },
  },
  methods: {
    formatDateOnly(date) {
      return new Date(date).toLocaleDateString();
    },
  },
};
</script>

<style scoped>
.responses-page {
  padding: 2rem 1rem;
  color: white;
  display: flex;
  flex-direction: column;
  align-items: center;
  min-height: 100vh;
}

h1 {
  font-size: 2rem;
  margin-bottom: 1.5rem;
  text-align: center;
}

/* Filter bar */
.filter-bar {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  justify-content: center;
  margin-bottom: 2rem;
  width: 100%;
  max-width: 1140px;
}

.filter-bar input[type="text"],
.filter-bar input[type="date"],
.filter-bar select {
  flex: 1 1 220px;
  background: white;
  color: #333;
  padding: 0.75rem 1rem;
  border-radius: 1rem;
  border: none;
  font-size: 1rem;
  min-width: 220px;
  max-width: 280px;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.1);
}

/* Responses grid */
.responses-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 2rem;
  justify-content: center;
  width: 100%;
  max-width: 1140px;
}

/* Ticket card */
.ticket-card {
  background-color: white;
  color: black;
  padding: 1.25rem;
  border-radius: 1rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.ticket-card-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 0.5rem;
}

.ticket-title {
  font-weight: bold;
  color: black;
}

.ticket-date {
  font-size: 0.9rem;
  color: black;
}

.ticket-message {
  margin-bottom: 0.5rem;
  color: black;
}

.ticket-status {
  font-style: italic;
  font-weight: 500;
  color: black;
}

.no-tickets {
  margin-top: 2rem;
  color: #f88;
  font-weight: bold;
}
</style>
